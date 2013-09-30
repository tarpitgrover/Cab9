using Cab9.Geography;
using Cab9.Hubs;
using Cab9.Model;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Hosting;

namespace Cab9.Common
{
    public class LiveDemo : IRegisteredObject 
    {
        private bool shuttingDown = false;
        private List<LocationProgress> progesses = new List<LocationProgress>();
        private Timer timer;
        private IHubContext TrackingHub = GlobalHost.ConnectionManager.GetHubContext<DriverHub>();
        private Random rand = new Random();

        public class LocationProgress
        {
            public int DriverID { get; set; }
            public Polyline Path { get; set; }
            public bool Reverse { get; set; }
            public int CurrentStep { get; set; }
        }

        public LiveDemo(TimeSpan startIn, TimeSpan period)
        {
            var drivers = Driver.Select(companyId: 1);
            foreach (var driver in drivers)
            {
                LocationProgress addition1 = new LocationProgress()
                {
                    DriverID = driver.ID,
                    Path = new Polyline(@"m}eyHd~rAkAAOpCEbE@dBZX~BEfAn@RlB?`CGdB????^]l@eAt@{@t@m@h@e@H}@?wC@yB?wC?oCGcDEkC@}BRkBxAqBrAU~BD`CBzB@\CX[^sDCmFAgD@iGB}CP_Fb@cEx@}C~@iA|Ak@tAWpAy@lA{AtAoBhA_BnAcBzAsAh@g@^Xp@hB`A|BhAfC|@rB|@jBv@fBr@pBv@`C`@tBRj@l@^B|AXnCp@tC^l@RTKj@Il@Jh@V^x@Oh@BbDhBbB|AvAhCpA|DfBtFnAhEpAjDlAhEbA|CzArExAnEjAtDz@jCfBlF|AlFjA|D^bGAhDx@fDnBtCnAv@zChEvAbAsBhE|A|HuAzDiBm@}@{A????????kB{Co@FWcAfA{BdAqB|AB~@Tb@{@hA}Cr@dA^pE`AhIzBbJdAtFj@|MaAtHkAzDGvBo@xJgA`Mm@nMiA`VgA~Ro@|P}A`NmE`K_GpGm@zMDz]}@vV{@~IoFzSqBvCkJwEqQwB{OjDuGjJcLtHsNwBuGqCwKqCaL_FwCw@WgJd@kSxCm[hDa_@`@u]NiPN{Ml@yPn@kQ?gQc@{PSaMFcSX{T\}YJcNTwPZkNFeCBmGNoOCqCiAK"),
                    Reverse = (rand.Next(0,2) == 1) ? true : false,
                };
                addition1.CurrentStep = rand.Next(0, addition1.Path.Points.Count - 1);
                progesses.Add(addition1);
            }

            timer = new Timer(OnTimerElapsed, null, startIn, period);
            HostingEnvironment.RegisterObject(this);
        }

        private void OnTimerElapsed(object state)
        {
            foreach (var item in progesses)
            {
                var send = new { 
                    DriverID = item.DriverID,
                    Point = item.Path.Points[item.CurrentStep]
                };

                //item.Driver.LastKnownPosition = item.Path.Points[item.CurrentStep];
                //item.Driver.Update();

                TrackingHub.Clients.All.PositionUpdate(send);
                if (item.Reverse)
                {
                    item.CurrentStep--;
                    if (item.CurrentStep == -1) item.CurrentStep = item.Path.Points.Count - 1;
                }
                else
                {
                    item.CurrentStep++;
                    if (item.CurrentStep == item.Path.Points.Count - 1) item.CurrentStep = 0;
                }
            }
        }

        public void Stop(bool immediate)
        {
            if (immediate)
            {
                timer.Dispose();
                HostingEnvironment.UnregisterObject(this);
            }
            else
            {
                timer.Dispose();
                HostingEnvironment.UnregisterObject(this);
            }
        }
    }
}