using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HMI_simulator
{
	public delegate void RecvSocketMsgDelegate(string msg);

	public class COMMUNICATOR
	{
		int PortNum = 2017;
		Thread MainThread = null;
		Socket sSocket = null;

		public RecvSocketMsgDelegate RecvMsgDel = null;
		string EncodingStr = "GB2312";

		public COMMUNICATOR(string encoding_str)
		{
			this.EncodingStr = encoding_str;
		}

		public void Start()
		{
			this.MainThread = new Thread(new ThreadStart(Com_Main));
			this.MainThread.Start();
		}

		public void Stop()
		{
			IPEndPoint ipep = new IPEndPoint(IPAddress.Loopback, this.PortNum);
			Socket cSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				cSocket.Connect(ipep);
				string sndStr = COM_QUIT_STR;
				byte[] sndBytes = Encoding.ASCII.GetBytes(sndStr);
				cSocket.Send(sndBytes);
				cSocket.Close();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.ToString());
			}
		}

		const string COM_QUIT_STR = "HMI_SIM_QUIT";

		void Com_Main()
		{
			IPEndPoint ipep = new IPEndPoint(IPAddress.Any, this.PortNum);
			this.sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.sSocket.Bind(ipep);
			this.sSocket.Listen(10);

			try
			{
				while (true)
				{
					Socket cSocket = this.sSocket.Accept();
					IPEndPoint clientip = (IPEndPoint)cSocket.RemoteEndPoint;
					Console.WriteLine("Connect with client:" + clientip.Address + " at port:" + clientip.Port);

					string recvStr = string.Empty;
					while (true)
					{
						byte[] recvBytes = new byte[1024];
						int bytes;
						bytes = cSocket.Receive(recvBytes, recvBytes.Length, 0);
						if (bytes > 0)
						{
							//Encoding shift_jis_encoding = Encoding.GetEncoding("Shift_JIS");
							//recvStr += shift_jis_encoding.GetString(recvBytes, 0, bytes);
							Encoding encoding = Encoding.GetEncoding(this.EncodingStr);
							recvStr += encoding.GetString(recvBytes, 0, bytes);
							Console.WriteLine("Server get message:{0}", recvStr);
							if (null != this.RecvMsgDel
								&& !recvStr.Equals(COM_QUIT_STR))
							{
								this.RecvMsgDel(recvStr);
							}
						}
						else
						{
							break;
						}
					}
					cSocket.Close();
					if (recvStr.Equals(COM_QUIT_STR))
					{
						this.sSocket.Close();
						break;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			finally
			{
				this.sSocket.Close();
			}
		}
	}
}
