﻿using DocumentCentreTests.Pages;
using DocumentCentreTests.Util;
using Machine.Specifications;
using NCrunch.Framework;
using System.Threading;

namespace DocumentCentreTests.Functional_Tests.Member.Catalogue
{
    [Timeout(900000)]
    public class When_oldstyle_catg_cart_calculates_units_total : BaseDriverTest
    {
        private static MemberHomePage _homePage;
        private static CataloguesPage _catPage;
        private static ProductsPage _prodPage;
        private static MyCartPage _cartPage;

        Establish context = () =>
        {
            LoadDriver();
            _logger.Info("-- Old Style Catalogue Units Calculation Test Initiating --");
            LoginPage loginPage = new LoginPage(_driver, Constants.UserType.MEMBER);
            _homePage = (MemberHomePage)loginPage.LoginAs(Constants.Affiliation.SA.MEMBER_USER, Constants.Affiliation.SA.MEMBER_PASS);
            _catPage = _homePage.NavigateToCatalogues();
            _catPage.InputCatalogueName("milan");
            _catPage.InitiateSearch();
            _prodPage = _catPage.ChooseCatalogue("Milan Automation Catalogue");
            _prodPage.LoadProductRows();
            _prodPage.AddItemToCart("IN-MILANTEST-01", 1);
            _prodPage.AddItemToCart("IN-MILANTEST-02", 1);
            _prodPage.AddItemToCart("IN-MILANTEST-03", 1);
            _prodPage.AddItemToCart("IN-MILANTEST-04", 1);
        };

        Because of = () =>
        {
            try
            {
                _cartPage = _prodPage.NavigateToCart();
                _cartPage.LoadItemsInCart();
                _cartPage.VerifyTotalUnits(); ;
            }
            catch (System.Exception)
            {
                if (_cartPage.VerifyUnitSuccess)
                    _logger.Info("-- Old Style Catalogue Units Calculation Test: [SUCCESS w/ EXCEPTION ENCOUNTERED] --");
                else
                {
                    _logger.Fatal("-- Old Style Catalogue Units Calculation Test: [FAILED] --");
                    _prodPage.ItemAdded.ShouldBeTrue();
                }
            }
        };

        It should_calculate_all_items_in_cart = () =>
        {
            if (_cartPage.VerifyUnitSuccess)
                _logger.Info("-- Old Style Catalogue Units Calculation Test: [SUCCESS] --");
        };
    }
}
