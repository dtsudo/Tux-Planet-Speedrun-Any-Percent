
namespace DTLibrary
{
	public interface IFileIO
	{
		void PersistData(int fileId, ByteList data);
		ByteList FetchData(int fileId);

		void PersistVersionedData(int fileId, int version, ByteList data);
		ByteList FetchVersionedData(int fileId, int version);
	}
}
