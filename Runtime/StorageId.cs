namespace DarkBestiary
{
    public struct StorageId
    {
        private readonly string m_StorageId;

        public StorageId(string storageId)
        {
            m_StorageId = storageId;
        }

        public override string ToString()
        {
            return m_StorageId;
        }
    }
}