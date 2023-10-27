namespace Core
{
	public interface IPlugin
	{
		string Name { get; }
		void Initialize();
		string DoPluginWork(string arg1, string arg2);
	}
}
