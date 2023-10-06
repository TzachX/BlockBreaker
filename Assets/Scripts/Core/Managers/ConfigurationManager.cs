using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using System.Threading.Tasks;

namespace BB
{
    /* 
     * The configuration manager class. A couple of notes:
     * 
     * 1. There is no need for multiple instances of configuration manager, so I made it static.
     * 
     * 2. There is no need for the configuration manager to derive from Monobehaviour since it
     *    doesn't require access to the scene or anything in it.
     * 
     * 3. I contemplated the use of singleton, however this will mean a class would have to 
     *    contain its instance. Since the GameManager class is existing/responsible
     *    only on things in the Game scene, I wouldn't want it to have access to the menu UI as well
     *    and change the text there when the fetch is done.
     *    
     * 4. The firebase api calls were taken from the official git of quickstart-unity.
     *    https://github.com/firebase/quickstart-unity/blob/master/remote_config/testapp/Assets/Firebase/Sample/RemoteConfig/UIHandler.cs
     */
    public static class ConfigurationManager
    {
        public static Action<BBConfig> OnSyncComplete; // Using an event to change the data (Also scalable since we can add more listeners in the future)
        public static bool fetchSucceeded = false;

        static ConfigurationManager() 
        {
            InitFirebase();
        }

        // Initializing firebase in the project
        private static void InitFirebase()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available) 
                {
                    SetDefault();
                }
                else
                {
                    Debug.Log("Could not resolve all Firebase dependencies");
                }
            });
        }

        // Setting default values to avoid crashes
        private static void SetDefault()
        {
            Dictionary<string, object> defaults = new();
            defaults.Add("BBConfig", "default");

            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults).ContinueWithOnMainThread(task =>
            {
                FetchConfig();
            });
        }

        // Fetching the remote config from firebase
        private static Task FetchConfig()
        {
            Task fetchTask = FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        // Handling the fetch result
        static void FetchComplete(Task fetchTask)
        {
            var info = FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case LastFetchStatus.Success:
                    Debug.Log("Fetch completed successfully!");
                    FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task => {
                        Debug.Log(String.Format("Remote data loaded and ready (last fetch time {0}).",
                                       info.FetchTime));
                        fetchSucceeded = true;
                        OnSyncComplete.Invoke(GetConfig<BBConfig>());
                    });

                    break;
                case LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case FetchFailureReason.Error:
                            Debug.Log("Fetch failed for unknown reason");
                            break;
                        case FetchFailureReason.Throttled:
                            Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    break;
                case LastFetchStatus.Pending:
                    Debug.Log("Latest Fetch call still pending.");
                    break;
            }
        }

        // Getting the value and converting it from json to a baseconfig type
        public static T GetConfig<T>() where T : BaseConfig
        {
            var configName = typeof(T).Name;
            var configResult = FirebaseRemoteConfig.DefaultInstance.GetValue(configName);
            return JsonConvert.DeserializeObject<T>(configResult.StringValue);
        }
    }

    // Defining the config types (Added base config for scalability)
    [Serializable]
    public abstract class BaseConfig
    {
        public bool isEnabled = true;
        public int configVer = 1;
    }

    public class BBConfig : BaseConfig
    {
        public string menuText;
    }
}
