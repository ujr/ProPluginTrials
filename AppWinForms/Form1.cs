using Core;

namespace AppWinForms
{
	public partial class Form1 : Form
	{
		private readonly IPlugin _plugin;

		public Form1(IPlugin plugin)
		{
			_plugin = plugin ?? throw new ArgumentNullException(nameof(plugin));

			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			_plugin.Initialize();
			var s = _plugin.DoPluginWork(@"C:\Work\Data\Esri\California.gdb", "Schools");

			base.OnLoad(e);
		}
	}
}