﻿using Gallio.Framework;
using Gallio.Model;
using MbUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;

namespace DocumentCentreTests.Pages
{
    /// <summary>
    /// Abstract class representing generic home page (can be either member, vendor, or group).
    /// Abstract functionality to share common return types for method returns/signatures.
    /// </summary>
    public abstract class HomePage
    {
        public abstract ViewOrdersPage NavigateToViewOrders();
    }
}
