﻿using System;
using System.Net;
using System.Threading.Tasks;


using FxSsh;
using FxSsh.Services;

namespace SshServerTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SshServer(new StartingInfo(IPAddress.Any, 2223));
            server.AddHostKey("ssh-rsa", "BwIAAACkAABSU0EyAAQAAAEAAQADKjiW5UyIad8ITutLjcdtejF4wPA1dk1JFHesDMEhU9pGUUs+HPTmSn67ar3UvVj/1t/+YK01FzMtgq4GHKzQHHl2+N+onWK4qbIAMgC6vIcs8u3d38f3NFUfX+lMnngeyxzbYITtDeVVXcLnFd7NgaOcouQyGzYrHBPbyEivswsnqcnF4JpUTln29E1mqt0a49GL8kZtDfNrdRSt/opeexhCuzSjLPuwzTPc6fKgMc6q4MBDBk53vrFY2LtGALrpg3tuydh3RbMLcrVyTNT+7st37goubQ2xWGgkLvo+TZqu3yutxr1oLSaPMSmf9bTACMi5QDicB3CaWNe9eU73MzhXaFLpNpBpLfIuhUaZ3COlMazs7H9LCJMXEL95V6ydnATf7tyO0O+jQp7hgYJdRLR3kNAKT0HU8enE9ZbQEXG88hSCbpf1PvFUytb1QBcotDy6bQ6vTtEAZV+XwnUGwFRexERWuu9XD6eVkYjA4Y3PGtSXbsvhwgH0mTlBOuH4soy8MV4dxGkxM8fIMM0NISTYrPvCeyozSq+NDkekXztFau7zdVEYmhCqIjeMNmRGuiEo8ppJYj4CvR1hc8xScUIw7N4OnLISeAdptm97ADxZqWWFZHno7j7rbNsq5ysdx08OtplghFPx4vNHlS09LwdStumtUel5oIEVMYv+yWBYSPPZBcVY5YFyZFJzd0AOkVtUbEbLuzRs5AtKZG01Ip/8+pZQvJvdbBMLT1BUvHTrccuRbY03SHIaUM3cTUc=");
            server.AddHostKey("ssh-dss", "BwIAAAAiAABEU1MyAAQAAG+6KQWB+crih2Ivb6CZsMe/7NHLimiTl0ap97KyBoBOs1amqXB8IRwI2h9A10R/v0BHmdyjwe0c0lPsegqDuBUfD2VmsDgrZ/i78t7EJ6Sb6m2lVQfTT0w7FYgVk3J1Deygh7UcbIbDoQ+refeRNM7CjSKtdR+/zIwO3Qub2qH+p6iol2iAlh0LP+cw+XlH0LW5YKPqOXOLgMIiO+48HZjvV67pn5LDubxru3ZQLvjOcDY0pqi5g7AJ3wkLq5dezzDOOun72E42uUHTXOzo+Ct6OZXFP53ZzOfjNw0SiL66353c9igBiRMTGn2gZ+au0jMeIaSsQNjQmWD+Lnri39n0gSCXurDaPkec+uaufGSG9tWgGnBdJhUDqwab8P/Ipvo5lS5p6PlzAQAAACqx1Nid0Ea0YAuYPhg+YolsJ/ce");
            server.ConnectionAccepted += server_ConnectionAccepted;

            server.Start();

            Task.Delay(-1).Wait();
        }

        static void server_ConnectionAccepted(object sender, Session e)
        {
            Console.WriteLine("Accepted a client.");

            e.ServiceRegistered += e_ServiceRegistered;
        }

        static void e_ServiceRegistered(object sender, SshService e)
        {
            var session = (Session)sender;
            Console.WriteLine("Session {0} requesting {1}.", BitConverter.ToString(session.SessionId).Replace("-", ""), e.GetType().Name);

            switch (e)
            {
                case UserauthService authService:
                    authService.Userauth += service_Userauth;
                    break;
                case ConnectionService connectionService:
                    connectionService.DirectTcpIpReceived += service_DirectTcpIpReceived;
                    connectionService.CommandOpened += service_CommandOpened;
                    break;

            }
        }

        private static void service_DirectTcpIpReceived(object sender, DirectTcpIpRequestedArgs e)
        {
            throw new NotImplementedException();
        }

        static void service_Userauth(object sender, UserauthArgs e)
        {
            switch (e)
            {
                case PKUserauthArgs pKUserauthArgs:
                    Console.WriteLine("Client {0} fingerprint: {1}.", pKUserauthArgs.KeyAlgorithm, pKUserauthArgs.Fingerprint);
                    break;
                case PasswordUserauthArgs passwordUserauthArgs:
                    var stop1 = true;
                    break;
                default:
                    var stop2 = true;
                    break;
            }

            e.Result = true;
        }

        static void service_CommandOpened(object sender, SessionRequestedArgs e)
        {
            Console.WriteLine("Channel {0} runs command: \"{1}\".", e.Channel.ServerChannelId, e.CommandText);

            var allow = true; // func(e.CommandText, e.AttachedUserauthArgs);

            //if (!allow)
            //    return;

            //var parser = new Regex(@"(?<cmd>git-receive-pack|git-upload-pack|git-upload-archive) \'/?(?<proj>.+)\.git\'");
            //var match = parser.Match(e.CommandText);
            //var command = match.Groups["cmd"].Value;
            //var project = match.Groups["proj"].Value;

            //var git = new GitService(command, project);

            //e.Channel.DataReceived += (ss, ee) => git.OnData(ee);
            //e.Channel.CloseReceived += (ss, ee) => git.OnClose();
            //git.DataReceived += (ss, ee) => e.Channel.SendData(ee);
            //git.CloseReceived += (ss, ee) => e.Channel.SendClose(ee);

            //git.Start();
        }
    }
}
