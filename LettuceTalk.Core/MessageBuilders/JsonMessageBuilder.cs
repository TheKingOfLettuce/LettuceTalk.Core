namespace LettuceTalk.Core.MessageBuilders;

/// <summary>
/// A <see cref="IMessageBuilder"/> that serializes and deserializes <see cref="Message"/>
/// by converting to JSON string and then to byte data following the LettuceTalk protocol
/// </summary>
public class JsonMessageBuilder : IMessageBuilder {

    /// <inheritdoc/>
    public Message FromData(byte[] data) {
        int messageCode = Convert.ToInt32(data[0]);
        byte[] jsonData = new byte[data.Length-1];
        Buffer.BlockCopy(data, 1, jsonData, 0, data.Length-1);
        
        return FromData(messageCode, jsonData);
    }

    /// <inheritdoc/>
    public Message FromData(int messageCode, byte[] data) {
        Type messageType = MessageFactory.GetMessageType(messageCode);
        string jsonString = System.Text.Encoding.UTF8.GetString(data);
        Message? message = (Message?)System.Text.Json.JsonSerializer.Deserialize(jsonString, messageType);
        if (message == null) {
            throw new Exception("Message failed to deserialize from JSON");
        }

        return message;
    }

    /// <inheritdoc/>
    public byte[] ToData(Message message) {
        string jsonString = System.Text.Json.JsonSerializer.Serialize(message, message.GetType());
        byte[] jsonData = System.Text.Encoding.UTF8.GetBytes(jsonString);
        byte messageCode = Convert.ToByte(MessageFactory.GetMessageCode(message));
        byte[] messageData = new byte[jsonData.Length+1];
        messageData[0] = messageCode;
        Buffer.BlockCopy(jsonData, 0, messageData, 1, jsonData.Length);

        return messageData;
    }
}