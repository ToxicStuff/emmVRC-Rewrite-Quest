﻿using System;
using emmVRC.Objects;
using System.Linq;
using emmVRC.Libraries;
using System.Collections;
using System.Collections.Generic;
using emmVRC.Objects.ModuleBases;
using System.Reflection;
using emmVRC.Utils;
using MelonLoader;

[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonInfo(typeof(emmVRC.Main), "emmVRC", "Quest", "The emmVRC Team (Rewrite XoX-Toxic)", "https://github.com/emmVRC/emmVRC-Revised")]

namespace emmVRC
{
    public class Main : MelonMod
    {
        public static bool Initialized = false;
        public static Main instance; 
        public static readonly AwaitProvider AwaitUpdate = new AwaitProvider();
        public override void OnApplicationStart()
        {
            // Load the config for emmVRC
            instance = this;
            Configuration.Initialize();
            foreach (MelonLoaderEvents listener in eventListeners)
            {
                try
                {
                    listener.OnApplicationStart();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnApplicationStart of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }


            MelonLoader.MelonCoroutines.Start(WaitForUiManagerInit());
        }
        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            foreach (MelonLoaderEvents listener in eventListeners)
            {
                try
                {
                    listener.OnSceneWasLoaded(buildIndex, sceneName);
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnSceneLoad of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }
        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            foreach (MelonLoaderEvents listener in eventListeners)
            {
                try
                {
                    listener.OnSceneWasInitialized(buildIndex, sceneName);
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnSceneInit of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName)
        {
            foreach (MelonLoaderEvents listener in eventListeners)
            {
                try
                {
                    listener.OnSceneWasUnloaded(buildIndex, sceneName);
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnSceneUnload of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }
        public override void OnUpdate()
        {
            AwaitUpdate.Flush();

            foreach (IWithUpdate listener in eventListenersWithUpdate)
            {
                try
                {
                    listener.OnUpdate();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnUpdate of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }

            if (!Initialized)
                return;
            // Check if resources have finished initializing
            if (Functions.Core.Resources.onlineSprite != null)
            {
                // If the user is new to emmVRC, present the emmVRC Welcome message
                if (!Configuration.JSONConfig.WelcomeMessageShown)
                {
                    Managers.emmVRCNotificationsManager.AddNotification(new Notification("Welcome to emmVRC!", null, "Welcome to emmVRC! For updates regarding the client, teasers for new features, and bug reports and support, join the Discord!", true, true, () => { Utils.ButtonAPI.GetQuickMenuInstance().AskConfirmOpenURL("https://discord.gg/emmVRC"); }, "Open Discord", "Open the invite to our Discord", true, null, "Dismiss", "Dismisses the notification"));
                    Configuration.WriteConfigOption("WelcomeMessageShown", true);
                }
            }

        }
        public override void OnFixedUpdate()
        {
            foreach (IWithFixedUpdate listener in eventListenersWithFixedUpdate)
            {
                try
                {
                    listener.OnFixedUpdate();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnFixedUpdate of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }
        public override void OnLateUpdate()
        {
            foreach (IWithLateUpdate listener in eventListenersWithLateUpdate)
            {
                try
                {
                    listener.LateUpdate();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnLateUpdate of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }

        public override void OnGUI()
        {
            foreach (IWithGUI listener in eventListenersWithGUI)
            {
                try
                {
                    listener.OnGUI();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running OnGUI of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }
        }
        public override void OnApplicationQuit()
        {

        }
        private static IEnumerator WaitForUiManagerInit()
        {
            while (VRCUiManager.field_Private_Static_VRCUiManager_0 == null)
                yield return null;

            OnUIManagerInit();
        }
        public static void OnUIManagerInit()
        {
            emmVRCLoader.Logger.LogDebug("UI manager initialized");

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            foreach (MelonLoaderEvents listener in eventListeners)
            {
                try
                {
                    listener.OnUiManagerInit();
                }
                catch (Exception ex)
                {
                    emmVRCLoader.Logger.LogError($"emmVRC encountered an exception while running UiManagerInit of \"{listener.GetType().FullName}\":\n" + ex.ToString());
                }
            }

            // At this point, if no errors have occured, emmVRC is done initializing
            watch.Stop();
            emmVRCLoader.Logger.Log("Initialization is successful in " + watch.Elapsed.ToString(@"ss\.f", null) + "s. Welcome to emmVRC!");
            emmVRCLoader.Logger.Log("You are running version " + Objects.Attributes.Version.ToString(3));
#if DEBUG
            // Debug actions go here
            Managers.DebugManager.DebugActions.Add(new Managers.DebugAction
            {
                ActionKey = UnityEngine.KeyCode.Alpha0,
                ActionAction = () =>
                {
                    WingPage page = new WingPage("Test", "Test");
                    emmVRCLoader.Logger.LogDebug("Building a button");
                    WingButton btn = new WingButton(ButtonAPI.GetQuickMenuInstance().transform.Find("Container/Window/Wing_Left/Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"), ButtonAPI.GetQuickMenuInstance().transform.Find("Container/Window/Wing_Right/Container/InnerContainer/WingMenu/ScrollRect/Viewport/VerticalLayoutGroup"), "emmVRC", () => { page.OpenLeftMenu(); }, "We built this city", Functions.Core.Resources.TabIcon);
                    btn.SetAction(() => { page.OpenRightMenu(); }, true);
                    WingButton testButton1 = new WingButton(page, "Test1", null, "Test 1");
                    WingButton testButton2 = new WingButton(page, "Test2", null, "Test 2");
                    WingButton testButton3 = new WingButton(page, "Test3", null, "Test 3");
                    WingButton testButton4 = new WingButton(page, "Test4", null, "Test 4");
                    WingButton testButton5 = new WingButton(page, "Test5", null, "Test 5");
                }
            });
#endif
            Initialized = true;
        }
        private readonly static IEnumerable<MelonLoaderEvents> eventListeners = typeof(Main).Assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(MelonLoaderEvents))).OrderBy((type) =>
          {
              PriorityAttribute priority = (PriorityAttribute)Attribute.GetCustomAttribute(type, typeof(PriorityAttribute));
              return priority == null ? 0 : priority.priority;
          }).Select(type => (MelonLoaderEvents)Activator.CreateInstance(type));

        private readonly static IEnumerable<IWithUpdate> eventListenersWithUpdate = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IWithUpdate).IsAssignableFrom(type) && type != typeof(IWithUpdate)).OrderBy((type) =>
           {
               PriorityAttribute priority = (PriorityAttribute)Attribute.GetCustomAttribute(type, typeof(PriorityAttribute));
               return priority == null ? 0 : priority.priority;
           }).Select(type => (IWithUpdate)Activator.CreateInstance(type));


        // Same situations as WithUpdate and these ones
        private readonly static IEnumerable<IWithFixedUpdate> eventListenersWithFixedUpdate = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IWithFixedUpdate).IsAssignableFrom(type) && type != typeof(IWithFixedUpdate)).OrderBy((type) =>
            {
                PriorityAttribute priority = (PriorityAttribute)Attribute.GetCustomAttribute(type, typeof(PriorityAttribute));
                return priority == null ? 0 : priority.priority;
            }).Select(type => (IWithFixedUpdate)Activator.CreateInstance(type));

        private readonly static IEnumerable<IWithLateUpdate> eventListenersWithLateUpdate = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IWithLateUpdate).IsAssignableFrom(type) && type != typeof(IWithLateUpdate)).OrderBy((type) =>
            {
                PriorityAttribute priority = (PriorityAttribute)Attribute.GetCustomAttribute(type, typeof(PriorityAttribute));
                return priority == null ? 0 : priority.priority;
            }).Select(type => (IWithLateUpdate)Activator.CreateInstance(type));

        private readonly static IEnumerable<IWithGUI> eventListenersWithGUI = Assembly.GetExecutingAssembly().GetTypes().Where(type => typeof(IWithGUI).IsAssignableFrom(type) && type != typeof(IWithGUI)).OrderBy((type) =>
            {
                PriorityAttribute priority = (PriorityAttribute)Attribute.GetCustomAttribute(type, typeof(PriorityAttribute));
                return priority == null ? 0 : priority.priority;
            }).Select(type => (IWithGUI)Activator.CreateInstance(type));

    }
}
