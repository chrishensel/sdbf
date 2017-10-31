using System.Collections.Generic;

namespace SimpleDosboxFrontend.Run
{
    class DosboxConfig
    {
        private static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _settings;

        static DosboxConfig()
        {
            var settings = new Dictionary<string, IReadOnlyDictionary<string, string>>();

            // SDL
            settings.Add("sdl", new Dictionary<string, string>()
            {
                { "fullscreen", "true" },                   // default was "false"
                { "fulldouble", "false" },
                { "fullresolution", "$ScreenSize$" },       // default was "original"
                { "windowresolution", "original" },
                { "output", "overlay" },                    // default was "surface"
                { "autolock", "true" },
                { "sensitivity", "100" },
                { "waitonerror", "true" },
                { "priority", "higher,normal" },
                { "mapperfile", "mapper-0.74.map" },
                { "usescancodes", "true" },
            });

            // Dosbox
            settings.Add("dosbox", new Dictionary<string, string>()
            {
                { "language", "" },
                { "machine", "svga_s3" },
                { "captures", "capture" },
                { "memsize","16" },
            });

            // Render
            settings.Add("render", new Dictionary<string, string>()
            {
                { "frameskip", "0" },
                { "aspect", "false" },
                { "scaler", "normal2x" },
            });

            // CPU
            settings.Add("cpu", new Dictionary<string, string>()
            {
                { "core", "auto" },
                { "cputype", "auto" },
                { "cycles", "max" },                        // default was "auto"
                { "cycleup", "10" },
                { "cycledown", "20" },
            });

            // Mixer
            settings.Add("mixer", new Dictionary<string, string>()
            {
                { "nosound", "false" },
                { "rate", "44100" },
                { "blocksize", "1024" },
                { "prebuffer", "20" },
            });

            // MIDI
            settings.Add("midi", new Dictionary<string, string>()
            {
                { "mpu401", "intelligent" },
                { "mididevice", "default" },
                { "midiconfig", "" },
            });

            // Soundblaster
            settings.Add("sblaster", new Dictionary<string, string>()
            {
                { "sbtype", "sb16" },
                { "sbbase", "220" },
                { "irq", "7" },
                { "dma", "1" },
                { "hdma", "5" },
                { "sbmixer", "true" },
                { "oplmode", "auto" },
                { "oplemu", "default" },
                { "oplrate", "44100" },
            });

            // GUS
            settings.Add("gus", new Dictionary<string, string>()
            {
                { "gus", "false" },
                { "gusrate", "44100" },
                { "gusbase", "240" },
                { "gusirq", "5" },
                { "gusdma", "3" },
                { "ultradir", @"C:\ULTRASND" },
            });

            // CPU speaker
            settings.Add("speaker", new Dictionary<string, string>()
            {
                { "pcspeaker", "true" },
                { "pcrate", "44100" },
                { "tandy", "auto" },
                { "tandyrate", "44100" },
                { "disney", "true" },
            });

            // Joystick
            settings.Add("joystick", new Dictionary<string, string>()
            {
                { "joysticktype", "auto" },
                { "timed", "true" },
                { "autofire", "false" },
                { "swap34", "false" },
                { "buttonwrap", "false" },
            });

            // Serial
            settings.Add("serial", new Dictionary<string, string>()
            {
                { "serial1", "dummy" },
                { "serial2", "dummy" },
                { "serial3", "disabled" },
                { "serial4", "disabled" },
            });

            // DOS
            settings.Add("dos", new Dictionary<string, string>()
            {
                { "xms", "true" },
                { "ems", "true" },
                { "umb", "true" },
                { "keyboardlayout", "auto" },
            });

            // IPX
            settings.Add("ipx", new Dictionary<string, string>()
            {
                { "ipx", "false" },
            });

            _settings = settings;
        }

        internal static IEnumerable<string> GetSections()
        {
            return _settings.Keys;
        }

        internal static IEnumerable<KeyValuePair<string, string>> GetValues(string sectionName)
        {
            return _settings[sectionName];
        }
    }
}
