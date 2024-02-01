﻿/*
    @app        : Screenpresso
    @repo       : https://github.com/Aetherinox/ScreenpressoKeygen
    @author     : Aetherinox
*/

#region "Using"
using System;
using System.Text;
using System.IO;
using System.Reflection;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Lng = ScreenpressoKG.Properties.Resources;
using Cfg = ScreenpressoKG.Properties.Settings;
#endregion

/*
    Attribute : Build Date
*/

#region "Class: Build Date Attribute"

[AttributeUsage( AttributeTargets.Assembly ) ]
    internal class BuildDateAttribute : Attribute
    {
        public BuildDateAttribute( string value )
        {
            DateTime = DateTime.ParseExact
            (
                value,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None
            );
        }

        public DateTime DateTime { get; }
    }

#endregion

/*
    Namespace
*/

namespace ScreenpressoKG
{

    /*
        Class : Helpers
    */

    class Helpers
    {

        #region "Define: Fileinfo"

            /*
                Define > File Name
                    utilized with logging
            */

            readonly static string log_file = "Helpers.cs";

        #endregion

        #region "Define: Classes"

            private AppInfo AppInfo             = new AppInfo( );

        #endregion

        #region "Define: Base Paths"

            private static string patch_launch_fullpath     = Process.GetCurrentProcess( ).MainModule.FileName;
            private static string patch_launch_dir          = Path.GetDirectoryName( patch_launch_fullpath );
            private static string patch_launch_exe          = Path.GetFileName( patch_launch_fullpath );
            private static string app_target_exe            = Cfg.Default.app_target_exe;

        #endregion

        #region "Define: Search Locations"

        /*
             Search Locations
        */

        private static string find_InAppData    = Path.Combine(
                                                    Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData ),
                                                    "Learnpulse\\Screenpresso",
                                                    app_target_exe
                                                );

        private static string find_InProg64     = Path.Combine(
                                                    Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles ),
                                                    "Learnpulse\\Screenpresso",
                                                    app_target_exe
                                                );

        private static string find_InProg86     = Path.Combine(
                                                    Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 ),
                                                    "Learnpulse\\Screenpresso",
                                                    app_target_exe
                                                );

        private static string find_InAppHome    = Path.Combine(
                                                    patch_launch_dir,
                                                    app_target_exe
                                                );

        #endregion

        #region "Method: (str) Find App Location"

            /*
                Find App

                find target application

                A file will be checked in the following order:
                    -   Screenpresso portable (same directory as patch exe)
                    -   Windows Environment Variable PATH
                    -   C:\Program Files
                    -   C:\Program Files (x86)
                    -   C:\Users\$USER\AppData\Local
                    -   Patcher exe directory (Where the patcher was executed from)
                    -   Powershell where command

                @return     str | full path ( dir + file.xxx )
            */

            public string FindApp( )
            {

                /*
                    looks for the portable app, this takes priority in case the user wants to activate the portable version
                */

                string[] drives         = System.IO.Directory.GetFiles( patch_launch_dir, "*Screenpresso_v*.exe");
                var i_filesFound        = drives.Count( );

                if ( i_filesFound > 0 )
                {
                    string found    = drives[0];
                    string dir      = Path.GetDirectoryName( found );

                    if ( Directory.Exists( dir ) )
                    {

                        if ( AppInfo.bIsDebug( ) )
                        {
                            MessageBox.Show(
                                string.Format( Lng.msgbox_debug_fpath_msg, app_target_exe, dir ), string.Format( Lng.msgbox_debug_fpath_title ),
                                MessageBoxButtons.OK, MessageBoxIcon.None
                            );
                        }

                        return found;
                    }
                }

                /*
                    Check for path inside Windows Environment Variables
                */

                String path         = Environment.GetEnvironmentVariable( "path" );
                String[] folders    = path.Split( ';' );

                foreach ( String folder in folders )
                {
                    if ( File.Exists( folder + app_target_exe ) )
                    {
                        if ( AppInfo.bIsDebug( ) )
                        {
                            MessageBox.Show(
                                string.Format( Lng.msgbox_debug_fpath_env_c1_msg, app_target_exe, folder ), string.Format( Lng.msgbox_debug_fpath_env_c1_title ),
                                MessageBoxButtons.OK, MessageBoxIcon.None
                            );
                        }

                        return folder + app_target_exe;
                    }
                    else if ( File.Exists( folder + "\\" + app_target_exe ) )
                    {
                        if ( AppInfo.bIsDebug( ) )
                        {
                            MessageBox.Show( string.Format( Lng.msgbox_debug_fpath_env_c2_msg, app_target_exe, folder ), string.Format( Lng.msgbox_debug_fpath_env_c2_title ),
                                MessageBoxButtons.OK, MessageBoxIcon.None
                            );
                        }

                        return folder + "\\" + app_target_exe;
                    }
                }

                /*
                    Program files 64
                        C:\Program Files
                */

                if ( File.Exists( find_InProg64 ) )
                {

                    if ( AppInfo.bIsDebug( ) )
                    {
                        MessageBox.Show(
                            string.Format( Lng.msgbox_debug_fpath_msg, app_target_exe, find_InProg64 ), string.Format( Lng.msgbox_debug_fpath_title ),
                            MessageBoxButtons.OK, MessageBoxIcon.None
                        );
                    }

                    return find_InProg64;
                }

                /*
                    Program files 86
                        C:\Program Files (x86)
                */

                if ( File.Exists( find_InProg86 ) )
                {

                    if ( AppInfo.bIsDebug( ) )
                    {
                        MessageBox.Show(
                            string.Format( Lng.msgbox_debug_fpath_msg, app_target_exe, find_InProg86 ), string.Format( Lng.msgbox_debug_fpath_title ),
                            MessageBoxButtons.OK, MessageBoxIcon.None
                        );
                    }

                    return find_InProg86;
                }

                /*
                    AppData
                        C:\Users\$USER\AppData\Local
                */

                if ( File.Exists( find_InAppData ) )
                {

                    if ( AppInfo.bIsDebug( ) )
                    {
                        MessageBox.Show(
                            string.Format( Lng.msgbox_debug_fpath_msg, app_target_exe, find_InAppData ), string.Format( Lng.msgbox_debug_fpath_title ),
                            MessageBoxButtons.OK, MessageBoxIcon.None
                        );
                    }

                    return find_InAppData;
                }

                /*
                   Patcher exe folder
                        This is where the exe patcher resides
                */

                if ( File.Exists( find_InAppHome ) )
                {

                    if ( AppInfo.bIsDebug( ) )
                    {
                        MessageBox.Show(
                            string.Format( Lng.msgbox_debug_fpath_msg, app_target_exe, find_InAppHome ), string.Format( Lng.msgbox_debug_fpath_title ),
                            MessageBoxButtons.OK, MessageBoxIcon.None
                        );
                    }

                    return find_InAppHome;
                }

                /*
                    Last Resort
                        Utilize powershell get-command to see if application is installed
                */

                string[] ps_query       = { "(get-command " + Cfg.Default.app_target_exe + ").Path" };
                string ps_result        = PowershellQ( ps_query );
                ps_result               = ps_result.Replace( @"\", @"\\" ).Replace( @"""", @"\""" );
                string target_where     = null;

                using ( var reader = new StringReader( ps_result ) )
                {
                    target_where = @reader.ReadLine( );
                }

                if ( File.Exists( target_where ) )
                {
                    if ( AppInfo.bIsDebug( ) )
                    {
                        MessageBox.Show(
                            string.Format( Lng.msgbox_debug_fpath_ps_msg, app_target_exe, target_where ),
                            string.Format( Lng.msgbox_debug_fpath_ps_title ),
                            MessageBoxButtons.OK, MessageBoxIcon.None
                        );
                    }

                    return target_where;
                }

                return String.Empty;
            }

        #endregion

        #region "Method: (str) Get Folder > ProgramFiles (Both)"

            /*
                ProgramFilesx86 directory
                    different way of checking for 32 vs 64 bit OS. Need it for special purposes VS the built-in functions.

                IntPtr.Size
                    4   = 32-bit
                    8   = 64-bit

                @return     : str
            */

            public static string ProgramFiles( )
            {
                if ( 4 == IntPtr.Size || ( !String.IsNullOrEmpty( Environment.GetEnvironmentVariable( "PROCESSOR_ARCHITEW6432" ) ) ) )
                    return Environment.GetEnvironmentVariable( "ProgramFiles(x86)" );

                return Environment.GetEnvironmentVariable( "ProgramFiles" );
            }

        #endregion

        #region "Method: (str) Get Folder > ProgramFilesx64"

            /*
                ProgramFiles64 directory

                @return     : str
            */

            public static string ProgramFilesx64( )
            {
                return Environment.GetEnvironmentVariable( "ProgramFiles" );
            }

        #endregion

        #region "Method: (str) Get Folder > ProgramFilesx86"

            /*
                return ProgramFiles86 directory
            */

            public static string ProgramFilesx86( )
            {
                return Environment.GetEnvironmentVariable( "ProgramFiles(x86)" );
            }

        #endregion

        #region "Method: (str) Exec Powershell Queries"

            /*
                Execute powershell query
                checks to see if a target file has been signed with x509 cert

                @param      : str query
                @return     : str
            */

            public string PowershellQ( string[] queries )
            {
                using ( PowerShell ps = PowerShell.Create( ) )
                {
                    for (int i = 0; i < queries.Length; i++) 
                    {
                        ps.AddScript( queries[ i ] );
                        Log.Send( log_file, new System.Diagnostics.StackTrace( true ).GetFrame( 0 ).GetFileLineNumber( ), "[ PSQ.Execute ]", String.Format( "{0}", queries[ i ] ) );
                    }

                    Collection<PSObject> PSOutput = ps.Invoke( );
                    StringBuilder sb = new StringBuilder( );

                    string resp = "";
                    foreach ( PSObject PSItem in PSOutput )
                    {
                        if ( PSItem != null )
                        {
                            Log.Send( log_file, new System.Diagnostics.StackTrace( true ).GetFrame( 0 ).GetFileLineNumber( ), "[ PSQ.Result ]", String.Format( "{0}", PSItem ) );
                            sb.AppendLine( PSItem.ToString( ) );
                        }
                    }

                    if ( ps.Streams.Error.Count > 0 )
                    {
                        resp += Environment.NewLine + string.Format( "{0} Error(s): ", ps.Streams.Error.Count );
                        foreach ( ErrorRecord err in ps.Streams.Error )
                                resp += Environment.NewLine + err.ToString();

                        Log.Send( log_file, new System.Diagnostics.StackTrace( true ).GetFrame( 0 ).GetFileLineNumber( ), "[ PSQ.Error ]", String.Format( "{0}", resp ) );
                    }

                    return sb.ToString( );
                }
            }

        #endregion

        #region "Method: DateTime"

            /*
                Linker Timestamp UTC

                used to help obtain the build date of the software.
                This method doesn't work if you compile your application with /deterministic flag 
            */

            public static DateTime GetLinkerTimestampUtc( Assembly assembly )
            {
                var location = assembly.Location;
                return GetLinkerTimestampUtc( location );
            }

            /*
                Get Linker Timestamp UTC

                used to help obtain the build date of the software.
                This method doesn't work if you compile your application with /deterministic flag 
            */

            public static DateTime GetLinkerTimestampUtc( string filePath )
            {
                const int peHeaderOffset            = 60;
                const int linkerTimestampOffset     = 8;
                var bytes                           = new byte[2048];

                using ( var file = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ) )
                {
                    file.Read( bytes, 0, bytes.Length );
                }

                var headerPos           = BitConverter.ToInt32( bytes, peHeaderOffset );
                var secondsSince1970    = BitConverter.ToInt32( bytes, headerPos + linkerTimestampOffset );
                var dt                  = new DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

                return dt.AddSeconds( secondsSince1970 );
            }

            /*
                Get Build Date/Time

                An alternative method to obtaining the build date of the software. 
                The functions above should not be used incombination with the one below.

                @usage  : DateTime build_date = Helpers.GetBuildDate( Assembly.GetExecutingAssembly( ) );
            */

            public static DateTime GetBuildDate( Assembly assembly )
            {
                const string buildver_meta_pref = "+build";
                var attribute       = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>( );

                if ( attribute?.InformationalVersion != null )
                {
                    var value       = attribute.InformationalVersion;
                    var index       = value.IndexOf( buildver_meta_pref );

                    if ( index > 0 )
                    {
                        value = value.Substring( index + buildver_meta_pref.Length );
                        if ( DateTime.TryParseExact( value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result ) )
                        {
                            return result;
                        }
                    }
                }

                return default;
            }

            public static DateTime GetBuild( Assembly assembly )
            {
                var attribute = assembly.GetCustomAttribute<BuildDateAttribute>( );
                return attribute?.DateTime ?? default( DateTime );
            }

        #endregion

        #region "Method: (bool) x509 > Check File Signed"

            /*
                x509 > File Is Signed
                checks to see if a target file has been signed with x509 cert

                @param      : str filepath
                @return     : bool
            */

            public static bool x509_FileSigned( string filepath )
            {
                var runeConfig = RunspaceConfiguration.Create( );
                using ( var rune = RunspaceFactory.CreateRunspace( runeConfig ) )
                {
                    rune.Open( );
                    using ( var pipe = rune.CreatePipeline())
                    {
                        string cmd_exe      = "Get-AuthenticodeSignature \"" + filepath + "\"";
                        pipe.Commands.AddScript( cmd_exe );
                        var results         = pipe.Invoke();
                        rune.Close          ( );
 
                        var sig             = results[ 0 ].BaseObject as Signature;

                        return sig == null || sig.SignerCertificate == null ? false : ( sig.Status != SignatureStatus.NotSigned );
                    }
                }
            }

        #endregion

        #region "Method: (bool) x509 > Thumbprint"

            /*
                x509 > Thumbprint
                returns the thumbprint for a target file if signed with x509 cert.

                @param      : str filepath
                @return     : str
            */

            public string x509_Thumbprint( string filepath )
            {
                if ( Helpers.x509_FileSigned( filepath ) )
                {

                    var runeConfig = RunspaceConfiguration.Create( );
                    using ( var rune = RunspaceFactory.CreateRunspace( runeConfig ) )
                    {
                        rune.Open( );
                        using ( var pipe = rune.CreatePipeline( ) )
                        {
                            string cmd_exe      = "Get-AuthenticodeSignature \"" + filepath + "\"";
                            pipe.Commands.AddScript( cmd_exe );
                            var results         = pipe.Invoke( );
                            rune.Close          ( );
 
                            var sig             = results[ 0 ].BaseObject as Signature;

                            if ( sig != null ) 
                                return sig.SignerCertificate.GetCertHashString( );

                            return "1";
                        }
                    }
                }

                return "0";
            }

        #endregion

        #region "Method: (bool) IsFileExe"

            /*
                Validate Executables
                returns if a target file is an executable.
                All executables begin with "MZ" or the hexadecimal "4D 5A"

                @arg        : str filepath
                @ret        : bool
            */

            static public bool IsExeFile( string filepath )
            {
                var bytesBegin = new byte[ 2 ];
                using( var fs = File.Open( filepath, FileMode.Open ) )
                {
                    fs.Read( bytesBegin, 0, 2 );
                }

                return Encoding.UTF8.GetString( bytesBegin ) == "MZ";
            }

        #endregion

    }
}
