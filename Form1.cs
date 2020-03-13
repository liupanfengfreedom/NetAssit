using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NetAssisit
{
    public delegate void UiCallback(object str);
    public partial class NetAssist : Form
    {
        TcpListener myListener;
        TcpSocketClient currenttcpsocket;
        TcpSocketClient mclient;

        UdpServer udpserver;
        Socket mclient_udp;
        IPEndPoint remoteEndPoint;
        public NetAssist()
        {
            InitializeComponent();
            mclient_udp = new Socket(AddressFamily.InterNetwork,
            SocketType.Dgram, ProtocolType.Udp);
            SynchronizationContext contex = SynchronizationContext.Current;
            new TcpSocketClient(this, mclient_udp, contex, (X) => {
                listBox4.Items.Add(remoteEndPoint.ToString() + ": " + DateTime.Now.ToString());
                listBox4.Items.Add(X as string);
            });
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void button1_Click(object sender, EventArgs e)
        {

                if (checkBox1.Checked)
                {
                    UnicodeEncoding asen = new UnicodeEncoding();

                    currenttcpsocket?.Send(asen.GetBytes(textBox2.Text));
                }
                else
                {
                    ASCIIEncoding asen = new ASCIIEncoding();

                    currenttcpsocket?.Send(asen.GetBytes(textBox2.Text));
                }
        }

        private void button2_Click(object sender, EventArgs e)//tcpserverlisten
        {
            if (button2.Text == "Stop")
            {
                myListener.Stop();
                currenttcpsocket?.abordthread();
                currenttcpsocket?.msocket.Close();
                button2.Text = "Listen";
                return;
            }
            string[] str = textBox1.Text.Split(':');
            if (str.Length != 2)
            {
                return;
            }
            try
            {
                int port = Int32.Parse(str[1]);
                IPAddress ipAd = IPAddress.Parse(str[0]);//local ip address  "172.16.5.188"
                myListener = new TcpListener(ipAd, port);//8001
                ///* Start Listeneting at the specified port */
                SynchronizationContext contex = SynchronizationContext.Current;
                myListener.Start();
                Thread thread = new Thread(Accept);
                thread.Start(contex);
            }
            catch (Exception ex)
            {
                return;
            }
            button2.Text = "Stop";

        }
        void Accept(object state)
        {
            while (true)
            {
                try
                {
                    Socket st = myListener.AcceptSocket();
                    currenttcpsocket = new TcpSocketClient(this, st, state, (X) =>
                    {
                        listBox1.Items.Add(st.RemoteEndPoint.ToString() + ": " + DateTime.Now.ToString());
                        listBox1.Items.Add(X as string);
                    });
                }
                catch (Exception e)
                {

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)//tcpclient
        {
            if (button4.Text == "Disconnect")
            {
                mclient.abordthread();
                mclient.msocket.Close();
                button4.Text = "Connect";
                return;
            }
            string[] str = textBox4.Text.Split(':');
            if (str.Length != 2)
            {
                return;
            }
            try
            {
                int port = Int32.Parse(str[1]);
                Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
                s.Connect(str[0], port);
                ///* Start Listeneting at the specified port */
                SynchronizationContext contex = SynchronizationContext.Current;
                mclient = new TcpSocketClient(this, s, contex, (X) =>
                {
                    listBox2.Items.Add(s.RemoteEndPoint.ToString() + ": " + DateTime.Now.ToString());
                    listBox2.Items.Add(X as string);
                });
                button4.Text = "Disconnect";
            }
            catch (Exception ex)
            {
                return;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (mclient!=null&&mclient.msocket.Connected)
            {
                if (checkBox1.Checked)
                {
                    UnicodeEncoding asen = new UnicodeEncoding();

                    mclient.Send(asen.GetBytes(textBox3.Text));
                }
                else
                {
                    ASCIIEncoding asen = new ASCIIEncoding();

                    mclient.Send(asen.GetBytes(textBox3.Text));
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (button6.Text == "Stop")
            {
                udpserver.abordthread();
                udpserver.msocket.Close();
                udpserver.msocket=null;
                button6.Text = "Listen";
                return;
            }
            string[] str = textBox6.Text.Split(':');
            if (str.Length != 2)
            {
                return;
            }
            try
            {
                Socket s = new Socket(AddressFamily.InterNetwork,
                SocketType.Dgram, ProtocolType.Udp);
                int port = Int32.Parse(str[1]);
                IPAddress ipAd = IPAddress.Parse(str[0]);//local ip address  "172.16.5.188"

                IPEndPoint m_LocalEndPoint = new IPEndPoint(ipAd, port);//27001
                s.Bind(m_LocalEndPoint);
                SynchronizationContext contex = SynchronizationContext.Current;
                udpserver = new UdpServer(this, s, contex, (X) =>
                {
                    listBox3.Items.Add(udpserver.remoteEP.ToString() + ": " + DateTime.Now.ToString());
                    listBox3.Items.Add(X as string);
                });

            }
            catch (Exception ex)
            {
                return;
            }
            button6.Text = "Stop";
        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                UnicodeEncoding asen = new UnicodeEncoding();

                udpserver?.Send(asen.GetBytes(textBox5.Text));
            }
            else
            {
                ASCIIEncoding asen = new ASCIIEncoding();

                udpserver?.Send(asen.GetBytes(textBox5.Text));

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string[] str = textBox8.Text.Split(':');
            if (str.Length != 2)
            {
                return;
            }
            int port = Int32.Parse(str[1]);
            IPAddress ipAd = IPAddress.Parse(str[0]);//local ip address  "172.16.5.188"

            remoteEndPoint = new IPEndPoint(ipAd, port);//27001

            if (checkBox1.Checked)
            {
                UnicodeEncoding asen = new UnicodeEncoding();
                mclient_udp.SendTo(asen.GetBytes(textBox7.Text), remoteEndPoint);
            }
            else
            {
                ASCIIEncoding asen = new ASCIIEncoding();             
                mclient_udp.SendTo(asen.GetBytes(textBox7.Text), remoteEndPoint);
            }
        }

        private void NetAssist_FormClosed(object sender, FormClosedEventArgs e)
        {
            myListener?.Stop();
            currenttcpsocket?.abordthread();
            currenttcpsocket?.msocket.Close();
            ///////////////////////////////////
            udpserver?.abordthread();
            udpserver?.msocket.Close();
        }
    }
}
