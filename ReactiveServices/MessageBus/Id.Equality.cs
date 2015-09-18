namespace ReactiveServices.MessageBus
{
    public partial class Id
    {
        public static bool operator ==(Id anId, Id anotherId)
        {
            if ((ReferenceEquals(anId, null)) &&
                (ReferenceEquals(anotherId, null)))
                return true;
            if (ReferenceEquals(anId, null)) 
                return false;
            if (ReferenceEquals(anotherId, null)) 
                return false;

            return anId.Equals(anotherId);
        }
        public static bool operator !=(Id anId, Id anotherId)
        {
            if ((ReferenceEquals(anId, null)) &&
                (ReferenceEquals(anotherId, null)))
                return false;
            if (ReferenceEquals(anId, null))
                return true;
            if (ReferenceEquals(anotherId, null))
                return true;

            return !anId.Equals(anotherId);
        }
        
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (GetType() != obj.GetType()) 
                return false;

            return SerializedValue == ((Id)obj).SerializedValue;
        }
    }
}
