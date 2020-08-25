﻿using CoreTweet;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;

namespace HHFO.Models
{
    public class Tweet: BindableBase, IDisposable, IEquatable<Tweet>
    {
        public Status Status { get; private set; }
        private CompositeDisposable Disposable = new CompositeDisposable();

        public Tweet(Status status)
        {
            Status = status;
        }


        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Status.Id == (obj as Tweet).Status.Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Status.ToString().GetHashCode();
        }

        public void Dispose()
        {
            this.Dispose();
        }

        public bool Equals([AllowNull] Tweet other)
        {
            return this.Status.Id == other?.Status?.Id;
        }
    }
}
