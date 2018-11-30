namespace Core {
    /// <summary>
    /// Message service receiver
    /// </summary>
    public interface IMessenger {
        /// <summary>
        /// Process a message
        /// </summary>
        /// <param name="message">Message object</param>
        Message Receive(Message message);

        /// <summary>
        /// Get the mask of which kind of message should be received
        /// <para>Mask is validate by bit, set the bit to 1 if the receiver wants to process it</para>
        /// </summary>
        /// <returns></returns>
        int Mask { get; }

        /// <summary>
        /// Get the status of messenger
        /// </summary>
        bool Awaking { get; }
    }
}
