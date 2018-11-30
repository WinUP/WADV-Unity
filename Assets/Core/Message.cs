namespace Core {
    /// <summary>
    /// Base system message
    /// </summary>
    public abstract class Message {

        /// <summary>
        /// Message tag
        /// </summary>
        public string Tag;

        /// <summary>
        /// Message mask
        /// </summary>
        public int Mask;
    }

    /// <inheritdoc />
    /// <summary>
    /// System message
    /// </summary>
    public class Message<T> : Message {
        /// <summary>
        /// Message content
        /// </summary>
        public T Content;

        /// <inheritdoc />
        /// <summary>
        /// Create new message with random ID and given content
        /// </summary>
        /// <param name="content">Message's content</param>
        public Message(T content) {
            Content = content;
        }
    }
}
