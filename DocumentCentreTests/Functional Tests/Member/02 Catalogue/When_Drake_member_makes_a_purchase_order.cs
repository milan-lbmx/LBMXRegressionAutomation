﻿using DocumentCentreTests.Pages;
using DocumentCentreTests.Util;
using Machine.Specifications;
using NCrunch.Framework;

namespace DocumentCentreTests.Functional_Tests.Member.Catalogue
{
    public class When_Drake_member_makes_a_purchase_order : BaseDriverTest
    {
        static MemberHomePage _homePage;
        static CataloguesPage _cataPage;
        static MyCartPage _cartPage;

        Establish context = () =>
        {
            LoadDriver();
            _logger.Info("-- Drake Purchase Order Test Initiating --");
            LoginPage loginPage = new LoginPage(_driver, Constants.UserType.MEMBER);
            _homePage = (MemberHomePage)loginPage.LoginAs(Constants.Affiliation.Drake.MEMBER_USER, Constants.Affiliation.Drake.MEMBER_PASS);
            _cataPage = _homePage.NavigateToCatalogues();
            _cataPage.InputCatalogueName("Knauf Insulation");
            _cataPage.InitiateSearch();
            _cartPage = _cataPage.ChooseDrakeCatalogue("Knauf Insulation Copy");
        };

        Because of = () =>
        {
            try
            {
                _cartPage.AddItemInline("EE100", Constants.Affiliation.Drake.USER);
                _cartPage.SendOrder();
            }
            catch(System.Exception)
            {
                _logger.Fatal("-- Drake PO Test: [FAILED] --");
            }
        };

        It should_complete_purchase_order = () =>
        {
            if (_cartPage.AlertSuccess.Equals(true))
            {
                _logger.Info("-- Drake PO Test: [PASSED] --");
                _cartPage.AlertSuccess.ShouldBeTrue();
            }
            else
            {
                _logger.Fatal("-- Drake PO Test: [FAILED] --");
                _cartPage.AlertSuccess.ShouldBeTrue();
            }
        };
    }
}
