

namespace ReactiveServices.MessageBus
{
    public class TopicId : Id<TopicId>
    {
        private static TopicId _default;
        /// <summary>
        /// Represents the default topic, used in case of no custom rounting
        /// </summary>
        public static TopicId Default
        {
            get { return _default ?? (_default = FromString("Default")); }
        }
        
        private static TopicId _none;
        /// <summary>
        /// Represents no topic at all, indicating no exchange should be created
        /// </summary>
        public static TopicId None
        {
            get { return _none ?? (_none = FromString("None")); }
        }
    }
}
