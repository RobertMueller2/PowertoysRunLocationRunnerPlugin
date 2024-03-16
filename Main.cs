using System.Diagnostics;
using System.IO;
using ManagedCommon;
using Wox.Plugin;

namespace Community.PowerToys.Run.Plugin.LocationRunner
{
    public class Main : IPlugin, IPluginI18n
    {
        public static string PluginID => "244a5a21-1ee7-4952-8d0d-6920f8d8c27c";

        public string Name => "LocationRunner";

        public string Description => @"Searches locations listed in %localappdata%\\LocationRunner\Locations.conf for executables and runs them.";

        private string? IconPath { get; set; }

        // TODO: the initial object is never used
        private Dictionary<string, Dictionary<string, string>> Categories { get; set; } = new Dictionary<string, Dictionary<string, string>>();

        private DateTime LastRefresh { get; set; }

        private PluginInitContext? Context { get; set; }

        private string ConfigFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocationRunner", "Locations.conf");

        public string GetTranslatedPluginDescription()
        {
            // TODO: localization
            return Description;
        }

        public string GetTranslatedPluginTitle()
        {
            // TODO: localization
            return Name;
        }

        private void Refresh()
        {
            if (!System.IO.File.Exists(ConfigFile))
            {
                File.Create(ConfigFile).Dispose();
                return;
            }

            if (LastRefresh != DateTime.MinValue && File.GetLastWriteTime(ConfigFile) < LastRefresh)
            {
                return;
            }

            Categories = new Dictionary<string, Dictionary<string, string>>();
            foreach (var line in System.IO.File.ReadAllLines(ConfigFile))
            {
                var l = line.Trim();
                if (l.Length == 0 || l.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var tokens = l.Split('=');
                string key;
                string location;
                switch (tokens.Length)
                {
                    case 1:
                        key = "DEF";
                        location = tokens[0];
                        break;
                    case 2:
                        key = tokens[0];
                        location = tokens[1];
                        break;
                    default:
                        // TODO: maybe error handling?
                        continue;
                }

                Dictionary<string, string> category;
                if (Categories.TryGetValue(key, out var cat))
                {
                    category = cat;
                }
                else
                {
                    category = new Dictionary<string, string>();
                    Categories[key] = category;
                }

                var files = System.IO.Directory.GetFiles(location, "*.exe", SearchOption.TopDirectoryOnly);
                foreach (var file in files)
                {
                    var basename = System.IO.Path.GetFileNameWithoutExtension(file);
                    category[basename] = file;
                }
            }
        }

        public void Init(PluginInitContext context)
        {
            Context = context;
            Context.API.ThemeChanged += OnThemeChanged;
            UpdateIconPath(context.API.GetCurrentTheme());
            Refresh();
        }

        public List<Result> Query(Query query)
        {
            Refresh();
            var list = new List<Result>();

            foreach (var cat in Categories)
            {
                var key = cat.Key;
                var searchTokens = query.Search.Split(' ', 2);
                if (query.Search.Length > 0 && !key.Contains(searchTokens[0], StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var location in cat.Value.Keys)
                {
                    if (query.Search.Length > 0 && searchTokens.Length > 1 && !location.Contains(searchTokens[1], StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    var file = cat.Value[location];
                    var dir = Directory.GetParent(file);

                    list.Add(new Result
                    {
                        Title = $"{key} - {location}",
                        SubTitle = $"{file} - Run", // TODO: localization
                        IcoPath = IconPath,
                        Action = _ =>
                        {
                            if (dir == null)
                            {
                                return false;
                            }

                            ProcessStartInfo startInfo = new ProcessStartInfo
                            {
                                FileName = file,
                                WorkingDirectory = dir.ToString(),
                            };

                            Process process = new Process
                            {
                                StartInfo = startInfo,
                            };

                            return process.Start();
                        },
                    });
                }
            }

            return list;
        }

        private void OnThemeChanged(Theme currentTheme, Theme newTheme)
        {
            UpdateIconPath(newTheme);
        }

        private void UpdateIconPath(Theme theme)
        {
            var t = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Light" : "Dark";
            IconPath = $@"Images\LocationRunner{t}.png";
        }
    }
}
