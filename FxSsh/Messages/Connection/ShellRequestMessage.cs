using System.Text;

namespace FxSsh.Messages.Connection
{
    public class ShellRequestMessage : ChannelRequestMessage
    {
        protected override void OnLoad(SshDataWorker reader)
        {
            /*
             byte      SSH_MSG_CHANNEL_REQUEST
             uint32    recipient channel
             string    "shell"
             boolean   want reply
            */
            base.OnLoad(reader);

            // if (reader.DataAvailable > -1)
            // {
            //     var stop = true;
            //     var read= reader.ReadString(Encoding.UTF8);
            //     var stop2 = true;
            // }
        }
    }
}