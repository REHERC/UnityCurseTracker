using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Harmony;
using Spectrum.API.Interfaces.Plugins;
using Spectrum.API.Interfaces.Systems;
using Spectrum.API.Storage;
using UnityEngine;

namespace UnityCurseTracker
{
    public class Jay2a : IPlugin
    {
        public void Initialize(IManager manager, string ipcIdentifier)
        {
            HarmonyInstance Harmony = HarmonyInstance.Create("com.REHERC.UnityCurseTracker");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public static void Unityd()
        {
            FileSystem fs = new FileSystem();

            string directory = $@"{fs.RootDirectory}\Screenshots\";

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filename = string.Format("{0} by {1} - {2}",G.Sys.GameManager_.Level_.Name_, G.Sys.GameManager_.LevelSettings_.LevelCreatorName_, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, ' ');
            }

            bool filealreadyexists = File.Exists($@"{directory}\{filename}.png");

            UnityEngine.Application.CaptureScreenshot($@"{directory}\{filename}.png");

            if (!filealreadyexists)
            {

            }
            string title = "Unityd - " + G.Sys.GameManager_.LevelSettings_.LevelName_;
            string description = "Screenshot saved.";
            NotificationBox.Notification n = new NotificationBox.Notification(title, description, NotificationBox.NotificationType.Car, 6f);
            NotificationBox.Show(n, false);
        }
    }

    [HarmonyPatch(typeof(CarLogic), "Start")]
    public class CarLogic__Start
    {
        static void Postfix(CarLogic __instance)
        {
            GameObject Car = __instance.gameObject;
            if (Car != null)
            {
                Car.AddComponent<UnitydTracker>();
            }
        }
    }

    public class UnitydTracker : MonoBehaviour
    {
        private bool UnitydThisFrame = false;
        private bool UnitydLastFrame = false;

        void Awake()
        {
            //Just to ensure the component is added and started
            Console.WriteLine("UnityTracker started");
        }

        void FixedUpdate()
        {
            List< RaycastHit> hits = new List<RaycastHit>();

            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.forward, 0.35f));
            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.forward * -1, 0.35f));
            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.up, 0.15f));
            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.up * -1, 0.15f));
            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.right, 0.25f));
            hits.AddRange<RaycastHit>(Physics.RaycastAll(transform.position, transform.right * -1, 0.25f));

            this.UnitydThisFrame = false;
            for (int i = 0; i < hits.Count; i++)
            {
                RaycastHit hit = hits[i];

                if (!(hit.transform.root.name == "LocalCar") && !(hit.transform.name == "LocalCar"))
                {
                    this.UnitydThisFrame = true;
                }
            }

            if (!this.UnitydLastFrame && this.UnitydThisFrame)
            {
                Jay2a.Unityd(); // I want this call to be triggered only after the unityd occurs but it is called every frame :(
            }

            this.UnitydLastFrame = this.UnitydThisFrame;
        }
    }
}
