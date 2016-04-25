// Written by Joe Zachary for CS 3500, November 2012
// Revised by Joe Zachary April 2016

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CustomNetworking
{
    /// <summary> 
    /// A StringSocket is a wrapper around a Socket.  It provides methods that
    /// asynchronously read lines of text (strings terminated by newlines) and 
    /// write strings. (As opposed to Sockets, which read and write raw bytes.)  
    ///
    /// StringSockets are thread safe.  This means that two or more threads may
    /// invoke methods on a shared StringSocket without restriction.  The
    /// StringSocket takes care of the synchronization.
    /// 
    /// Each StringSocket contains a Socket object that is provided by the client.  
    /// A StringSocket will work properly only if the client refrains from calling
    /// the contained Socket's read and write methods.
    /// 
    /// If we have an open Socket s, we can create a StringSocket by doing
    /// 
    ///    StringSocket ss = new StringSocket(s, new UTF8Encoding());
    /// 
    /// We can write a string to the StringSocket by doing
    /// 
    ///    ss.BeginSend("Hello world", callback, payload);
    ///    
    /// where callback is a SendCallback (see below) and payload is an arbitrary object.
    /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
    /// successfully written the string to the underlying Socket, or failed in the 
    /// attempt, it invokes the callback.  The parameters to the callback are a
    /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
    /// the Exception that caused the send attempt to fail.
    /// 
    /// We can read a string from the StringSocket by doing
    /// 
    ///     ss.BeginReceive(callback, payload)
    ///     
    /// where callback is a ReceiveCallback (see below) and payload is an arbitrary object.
    /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
    /// string of text terminated by a newline character from the underlying Socket, or
    /// failed in the attempt, it invokes the callback.  The parameters to the callback are
    /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
    /// string or the Exception will be non-null, but nor both.  If the string is non-null, 
    /// it is the requested string (with the newline removed).  If the Exception is non-null, 
    /// it is the Exception that caused the send attempt to fail.
    /// </summary>

    public class StringSocket
    {
        private static Encoding encoding;
        /// <summary>
        /// The type of delegate that is called when a send has completed.
        /// </summary>
        public delegate void SendCallback(Exception e, object payload);

        /// <summary>
        /// The type of delegate that is called when a receive has completed.
        /// </summary>
        public delegate void ReceiveCallback(String s, Exception e, object payload);

        // Underlying socket
        private Socket socket;



        // Text that has been received from the client but not yet dealt with
        private string incoming;



        /// <summary>
        /// Object used to synchronize sends to the clients
        /// </summary>
        private readonly object sendSync = new object();

        /// <summary>
        /// Object used to synchronize receives from the clients
        /// </summary>
        private readonly object readSync = new object();

        /// <summary>
        /// Queue that stores all impending receives, keeping them in order that they were received
        /// </summary>
        private Queue<ReceiveObject> recQueue;

        /// <summary>
        /// Queue that stores all pending sends, keeping them in order that they were received
        /// </summary>
        private Queue<SendObject> sendQueue;

        /// <summary>
        /// Queue that stores all messages that have been parsed from the receives from the clients.
        /// </summary>
        private Queue<string> messages;

        /// <summary>
        /// integer that keeps track of how many bytes have been sent so far in each send
        /// </summary>
        private int AllOutBytes;

        /// <summary>
        /// Creates a StringSocket from a regular Socket, which should already be connected.  
        /// The read and write methods of the regular Socket must not be called after the
        /// StringSocket is created.  Otherwise, the StringSocket will not behave properly.  
        /// The encoding to use to convert between raw bytes and strings is also provided.
        /// </summary>
        public StringSocket(Socket s, Encoding e)
        {
            
            socket = s;
            recQueue = new Queue<ReceiveObject>();
            sendQueue = new Queue<SendObject>();
            messages = new Queue<string>();

            encoding = e;
            incoming = "";

        }

        /// <summary>
        /// Shuts down and closes the socket.  No need to change this.
        /// </summary>
        public void Shutdown()
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// We can write a string to a StringSocket ss by doing
        /// 
        ///    ss.BeginSend("Hello world", callback, payload);
        ///    
        /// where callback is a SendCallback (see above) and payload is an arbitrary object.
        /// This is a non-blocking, asynchronous operation.  When the StringSocket has 
        /// successfully written the string to the underlying Socket, or failed in the 
        /// attempt, it invokes the callback.  The parameters to the callback are a
        /// (possibly null) Exception and the payload.  If the Exception is non-null, it is
        /// the Exception that caused the send attempt to fail. 
        /// 
        /// This method is non-blocking.  This means that it does not wait until the string
        /// has been sent before returning.  Instead, it arranges for the string to be sent
        /// and then returns.  When the send is completed (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginSend
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginSend must take care of synchronization instead.  On a given StringSocket, each
        /// string arriving via a BeginSend method call must be sent (in its entirety) before
        /// a later arriving string can be sent.
        /// </summary>
        public void BeginSend(String s, SendCallback callback, object payload)
        {
            lock (sendSync)
            {
                sendQueue.Enqueue(new SendObject(s, callback, payload));
                if (sendQueue.Count == 1)
                {
                    sendAllQueue();

                }

            }

        }

        /// <summary>
        /// method that is used to encode the outgoing message, and then start the send.  When the send is done, the MessageSent method is called.
        /// </summary>
        private void sendAllQueue()
        {
            byte[] OutBytes = encoding.GetBytes(sendQueue.Peek().message);
            socket.BeginSend(OutBytes, 0, OutBytes.Length,
                                 SocketFlags.None, MessageSent, OutBytes);
        }

        /// <summary>
        /// Called when a message has been successfully sent
        /// </summary>
        private void MessageSent(IAsyncResult result)
        {
            // Find out how many bytes were actually sent
            byte[] OutBytes = (byte[])result.AsyncState;
            int bytesSent = socket.EndSend(result);

            if (bytesSent == 0)
            {
                socket.Close();
            }
            else
            {
                AllOutBytes += bytesSent;
            }

            if (AllOutBytes == OutBytes.Length)
            {
                lock (sendSync)
                {
                    SendObject sentItem = sendQueue.Dequeue();

                    AllOutBytes = 0;

                    ThreadPool.QueueUserWorkItem(x => sentItem.sendObj(null, sentItem.sendPayload));

                    if (sendQueue.Count > 0)
                    {
                        sendAllQueue();
                    }
                }
            }

            else
            {
                socket.BeginSend(OutBytes, bytesSent, OutBytes.Length - bytesSent, SocketFlags.None, MessageSent, OutBytes);
            }




        }



        /// <summary>
        /// We can read a string from the StringSocket by doing
        /// 
        ///     ss.BeginReceive(callback, payload)
        ///     
        /// where callback is a ReceiveCallback (see above) and payload is an arbitrary object.
        /// This is non-blocking, asynchronous operation.  When the StringSocket has read a
        /// string of text terminated by a newline character from the underlying Socket, or
        /// failed in the attempt, it invokes the callback.  The parameters to the callback are
        /// a (possibly null) string, a (possibly null) Exception, and the payload.  Either the
        /// string or the Exception will be null, or possibly boh.  If the string is non-null, 
        /// it is the requested string (with the newline removed).  If the Exception is non-null, 
        /// it is the Exception that caused the send attempt to fail.  If both are null, this
        /// indicates that the sending end of the remote socket has been shut down.
        /// 
        /// This method is non-blocking.  This means that it does not wait until a line of text
        /// has been received before returning.  Instead, it arranges for a line to be received
        /// and then returns.  When the line is actually received (at some time in the future), the
        /// callback is called on another thread.
        /// 
        /// This method is thread safe.  This means that multiple threads can call BeginReceive
        /// on a shared socket without worrying around synchronization.  The implementation of
        /// BeginReceive must take care of synchronization instead.  On a given StringSocket, each
        /// arriving line of text must be passed to callbacks in the order in which the corresponding
        /// BeginReceive call arrived.
        /// 
        /// Note that it is possible for there to be incoming bytes arriving at the underlying Socket
        /// even when there are no pending callbacks.  StringSocket implementations should refrain
        /// from buffering an unbounded number of incoming bytes beyond what is required to service
        /// the pending callbacks.
        /// </summary>
        public void BeginReceive(ReceiveCallback callback, object payload, int length = 0)
        {
            lock (readSync)
            {
                recQueue.Enqueue(new ReceiveObject(callback, payload));

                if (recQueue.Count == 1)
                {
                    ReceiveAllQueue();
                }

            }
        }

        /// <summary>
        /// Method that is used to determine when the message received is a full message, and when so, calls the callback for the message received.
        /// if there are still receives pending, then the socket begins receives, and calls receivedBytes when the receive is done.
        /// </summary>
        private void ReceiveAllQueue()
        {

            int index;
            while ((index = incoming.IndexOf('\n')) >= 0)
            {
                //one message segment
                String Message = incoming.Substring(0, index);
                if (Message.EndsWith("\r"))
                {
                    Message = Message.Substring(0, index - 1);
                }
                messages.Enqueue(Message);
                incoming = incoming.Substring(index + 1);
            }
            while (recQueue.Count > 0 && messages.Count > 0)
            {
                lock (readSync)
                {
                    ReceiveObject goodreceive = recQueue.Dequeue();
                    string goodmessage = messages.Dequeue();
                    ThreadPool.QueueUserWorkItem(x => goodreceive.receiveObject(goodmessage, null, goodreceive.RecPayload));
                }
            }
            if (recQueue.Count > 0)
            {
                //bounded buffer. no DOSing
                byte[] buffer = new byte[1024];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedBytes, buffer);
            }
        }

        /// <summary>
        /// Method that determines how many bytes have been receive, and if bytes have been received, then gets the string of the bytes received
        /// Afterwards, calls the receiveAllQueue method.
        /// </summary>
        /// <param name="ar">The result from the beginreceive on the socket</param>
        private void ReceivedBytes(IAsyncResult ar)
        {
            //the actual bytes received
            byte[] buffer = (byte[])(ar.AsyncState);
            //get number of bytes received so far
            int bytesIn = socket.EndReceive(ar);
            if (bytesIn == 0)
            {
                socket.Close();
            }
            lock (readSync)
            {
                incoming += encoding.GetString(buffer, 0, bytesIn);
                ReceiveAllQueue();
            }
        }
       


        /// <summary>
        /// Class that is used to keep all properties of a Send together.  
        /// This class is used in the send queue and for setting up the callback for the sends.
        /// </summary>
        internal class SendObject
        {
            /// <summary>
            /// Keeps the message that is sent to the client with the relevant send
            /// </summary>
            public string message;

            /// <summary>
            /// The call back delegate method that is associated with the relevant send
            /// </summary>
            public SendCallback sendObj;

            /// <summary>
            /// object that is sent as a parameter to the callback method associated with the send
            /// </summary>
            public object sendPayload;

            /// <summary>
            /// Constructor of the sendObject that sets up the instance variables of the sendobject.
            /// </summary>
            /// <param name="mes">Message of the send</param>
            /// <param name="obj">Callback method of the send</param>
            /// <param name="pay">payload object of the relevant send</param>
            public SendObject(string mes, SendCallback obj, object pay)
            {
                message = mes;

                sendObj = obj;

                sendPayload = pay;
            }
        }

        /// <summary>
        /// Class that is used to keep all the relevant parts of a Receive togther. 
        /// This class is used in the receive queue and for getting all the relevant parts of a receive when setting up the callback
        /// </summary>
        internal class ReceiveObject
        {
            /// <summary>
            /// callback method that is associated with the particular receive
            /// </summary>
            public ReceiveCallback receiveObject;

            /// <summary>
            /// object that is used with the callback method associated with the receive
            /// </summary>
            public object RecPayload;

            /// <summary>
            /// constructor for the class, sets up the instance variables to equal the relvant parts of the receive
            /// </summary>
            /// <param name="recobj">Callback method of the receive</param>
            /// <param name="payload"> object that is passed to the callback method</param>
            public ReceiveObject(ReceiveCallback recobj, object payload)
            {
                receiveObject = recobj;
                RecPayload = payload;
            }
        }
    }
}