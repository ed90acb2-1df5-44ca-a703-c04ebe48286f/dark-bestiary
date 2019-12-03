namespace DarkBestiary
{
    public struct StorageId
    {
        private readonly string storageId;

        public StorageId(string storageId)
        {
            this.storageId = storageId;
        }

        public override string ToString()
        {
            return this.storageId;
        }
    }
}