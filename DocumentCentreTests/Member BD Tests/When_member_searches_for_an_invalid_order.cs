﻿using DocumentCentreTests.Pages;
using DocumentCentreTests.Util;
using Machine.Specifications;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentCentreTests.Member_BD_Tests
{
    [Subject(typeof(LoginPage))]
    public class When_member_searches_for_an_invalid_order : BaseDriverTest
    {
        static ViewOrdersPage _voPage;
        static HomePage _homePage;

        Establish context = () =>
        {
            LoadDriver();
            _logger.Info("-- Member Invalid Order Search Test Initiating --");
            LoginPage loginPage = new LoginPage(_driver, "member");
            _homePage = loginPage.LoginAs(Constants.MEM_PORTAL_USER, Constants.MEM_PORTAL_PASS);
        };

        Because of = () =>
        {
            _voPage = _homePage.NavigateToOrders("View Orders");
            _voPage.InputPurchaseOrder(Constants.INVALID_PO);
            _voPage.InitiateSearch();
            _voPage.CheckFirstRow();
        };


        It should_show_no_orders_found = () =>
        {
            if (_voPage.FirstTableElem.Text.Equals(Constants.ORDER_ERROR_MSG))
            {
                _logger.Info("-- Member Invalid Order Search Test: [PASSED] --");
                _voPage.FirstTableElem.Text.ShouldEqual(Constants.ORDER_ERROR_MSG);
            }
            else
            {
                _logger.Fatal("-- Member Invalid Order Search Test: [FAILED] --");
                _voPage.FirstTableElem.Text.ShouldEqual(Constants.ORDER_ERROR_MSG);
            }
        };
    }
}
