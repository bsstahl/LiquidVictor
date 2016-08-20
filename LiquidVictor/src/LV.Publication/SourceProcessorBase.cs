﻿using LV.Publication.Entities;
using LV.Publication.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication
{
    public delegate void StartEventHandler(object sender, EventArgs args);


    public abstract class SourceProcessorBase : ISourceProcessor
    {
        private bool StopRequested { get; set; }

        #region Constructors

        public SourceProcessorBase(Entities.Source source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            this.Id = Guid.NewGuid();
            this.AttemptTimeoutMs = source.AttemptTimeoutMs;
            this.IsActive = false;
            this.Config = source;
        }

        #endregion  

        #region ISourceProcessor Methods

        public Guid Id { get; private set; }

        public DateTime LastAttempt { get; private set; }

        public long AttemptTimeoutMs { get; private set; }

        public bool IsActive { get; private set; }

        public Source Config { get; private set; }


        public void Start()
        {
            this.StopRequested = false;
            this.IsActive = true;
            this.LastAttempt = DateTime.Now;
            Task.Factory.StartNew(() => Process());
        }

        private void Process()
        {
            while (!this.StopRequested)
            {
                if (this.IsActive)
                    DoWork(this.Config);
            }
        }

        public abstract void DoWork(Entities.Source source);

        public void Stop()
        {
            this.StopRequested = true;
            this.IsActive = false;
        }

        public bool Pause()
        {
            bool result = this.IsActive;
            if (this.IsActive)
                this.IsActive = false;
            return result;
        }

        public bool Unpause()
        {
            bool result = !this.IsActive;
            if (!this.IsActive)
                this.IsActive = true;
            return result;
        }

        #endregion

        #region Events

        public event StartEventHandler Started;

        protected virtual void OnStart(EventArgs e)
        {
            if (Started != null)
                Started(this, e);
        }

        #endregion

    }
}
