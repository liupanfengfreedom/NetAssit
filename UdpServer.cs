using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace NetAssisit
{
    class UdpServer
    {
        const int BUFFER_SIZE = 65536;
        const int SENDBUFFER_SIZE = 4096;
        byte[] sendbuffer = new byte[SENDBUFFER_SIZE];
        public byte[] receivebuffer = new byte[BUFFER_SIZE];
        public Socket msocket;
        NetAssist mform;
        UiCallback maction;
        Thread receivethread;
        public EndPoint remoteEP;
        public UdpServer(NetAssist form, Socket socket, object state, UiCallback action)
        {
            maction = action;
            mform = form;
            msocket = socket;
            receivethread = new Thread(ReceiveLoop);
            receivethread.Start(state);
        }
        public void Send(byte[] bytearray)
        {
            msocket?.SendTo(bytearray, remoteEP);
        }
        void ReceiveLoop(object state)
        {
            remoteEP = new IPEndPoint(1, 1);
            while (true)
            {
                try
                {
                    Array.Clear(receivebuffer, 0, receivebuffer.Length);
                    int size = msocket.ReceiveFrom(receivebuffer, ref remoteEP);
                    if (size == 0)//disconnected
                    {
                        break;
                    }
                    string receivestring;
                    if (mform.checkBox1.Checked)
                    {
                        receivestring = System.Text.Encoding.Unicode.GetString(receivebuffer);

                    }
                    else
                    {
                        receivestring = System.Text.Encoding.UTF8.GetString(receivebuffer);
                    }
                    SynchronizationContext contex = state as SynchronizationContext;
                    contex.Post((X) => { maction.Invoke(X); }, receivestring);
                }
                catch (SocketException)
                {

                }
            }

        }
        public void abordthread()
        {
            receivethread.Abort();
        } 
    }
}
