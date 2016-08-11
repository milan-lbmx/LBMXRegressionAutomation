﻿using DocumentCentreTests.Pages;
using DocumentCentreTests.Util;
using Machine.Specifications;

namespace DocumentCentreTests.Functional_Tests.Member.Login
{
    public class When_member_logs_in_with_valid_creds : BaseDriverTest
    {
        static LoginPage _loginPage;

        Establish context = () =>
        {
            LoadDriver();
            _logger.Info("-- Member Valid Login Test Initiating --");
            _loginPage = new LoginPage(_driver, "member");
        };

        Because of = () => _loginPage.LoginAs(Constants.SA_MEMBER_USER, Constants.SA_MEMBER_PASS);

        It should_have_successfully_logged_in = () =>
        {
            if (_loginPage.LoginSuccess)
            {
                _logger.Info("-- Member Valid Login Test: [PASSED] --");
                _loginPage.LoginSuccess.ShouldBeTrue();
            }
            else
            {
                _logger.Fatal("-- Member Valid Login Test: [FAILED] --");
                _loginPage.LoginSuccess.ShouldBeTrue();
            }
        };
    }
}