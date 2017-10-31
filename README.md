# sdbf
A very simple DOSBox-frontend that runs on Windows and Linux.

# Overview
This application is a simple DOSBox frontend. It uses a simple file format for profiles and generates conf files out of them.

The intent of this application is to only offer a minimalistic interface for starting DOSBox games and omitting features that are less or never used.

Also, platform support is very important. This program runs exactly the same on Windows and Unix-based OS via Mono.

# Profiles format
The program uses a simple XML-file for representing a profile.

```xml
<?xml version="1.0" encoding="utf-8"?>
<profile>
  <name>Doom</name>
  <paths>
    <path mount-as="C">@../../Games/doom</path>
  </paths>
  <run>DOOM.EXE</run>
</profile>
```

Example content of *Doom.sdbpx*

The application generates an appropriate DOSBox-configuration file (.conf) out of it if missing or if the profile has been modified.

## Overriding configuration values
You may need to override DOSBox-configuration values. In this case, you can add the *<overrides/>* element to the profile and set the values as follows.

```xml
<overrides>
    <item name="SECTION.NAME">VALUE</item>
</overrides>
```

For example, if you want to set the value of the **output** key from section **sdl** to **direct3d**, you can do so with:

```xml
<item name="sdl.output">direct3d</item>
```

The pattern is the same for any configuration except *autorun*.

# Configuration file

There is a configuration file called **SimpleDosboxFrontend.exe.config** which controls some platform-specific settings.

The configuration file may look like this:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<configSections>
		<sectionGroup name="applicationSettings" type="System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
			<section name="SimpleDosboxFrontend.Properties.Settings" type="System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
		</sectionGroup>
	</configSections>
	<applicationSettings>
		<SimpleDosboxFrontend.Properties.Settings>
			<setting name="DOSBoxExecutablePathWindows" serializeAs="String">
				<value>..\dosbox.exe</value>
			</setting>
			<setting name="DOSBoxExecutablePathUnix" serializeAs="String">
				<value>dosbox</value>
			</setting>
			<setting name="PortableMode" serializeAs="String">
				<value>False</value>
			</setting>
			<setting name="PortableModeProfileDir" serializeAs="String">
				<value>profiles</value>
			</setting>
		</SimpleDosboxFrontend.Properties.Settings>
	</applicationSettings>
</configuration>
```

## Settings

### DOSBoxExecutablePathWindows

This path is only important on Windows-based OS. It tells the application which DOSBox application to use.

### DOSBoxExecutablePathUnix

This path is only important on Linux-based OS. It tells the application which **command** to use.

### PortableMode

Setting this to false will by default search for profiles in the **My Documents** folder. This differs on Windows and Linux.

If in doubt, set this to **True** and set the profile directory in the next setting.

### PortableModeProfileDir

If "PortableMode" is set to "True", then this controls which directory will be used to search for profiles. This is relative to the application directory.