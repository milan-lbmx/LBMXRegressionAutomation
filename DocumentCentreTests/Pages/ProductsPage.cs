﻿using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using DocumentCentreTests.Util;
using NLog;
using System.Threading;
using DocumentCentreTests.Models;

namespace DocumentCentreTests.Pages
{
    public class ProductsPage
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private IWebDriver _driver;

        // product data structures
        internal List<Product> _products;
        internal List<Product> _prodsInCart;
        internal IList<IWebElement> _productVariants;
        internal IList<IWebElement> _productQtyUp;
        internal IList<IWebElement> _productQtyDown;
        internal IList<IWebElement> _productQtyLocators;
        internal IList<IWebElement> _productUpdateBtns;
        internal IList<IWebElement> _productRows;

        // physical elements (locators) on page
        private IWebElement ProductsTable;
        private IWebElement ReportsDropdown;
        private IWebElement SaveDraftButton;
        private IWebElement SearchBar;
        private IWebElement SearchButton;
        private IWebElement AdvancedSearchLinktext;
        private IWebElement GridViewButton;
        private IWebElement TilesViewButton;
        private IWebElement MyCartButton;
        
        #region Reports Dropdown Options
        private IWebElement DLCatAsPDFOption;
        private IWebElement DLCatAsExcelOption;
        private IWebElement OrderReportOption;
        private IWebElement SummaryOption;
        #endregion

        internal bool ItemAdded { get; private set; }
        internal bool PDFDownloaded { get; private set; }
        public ProductsPage(IWebDriver driver)
        {
            #region Assigning Accessors
            _driver = driver;
            ItemAdded = false;
            PDFDownloaded = false;
            ProductsTable = HelperMethods.FindElement(this._driver, Constants.SearchType.XPATH, "//tbody");
            ReportsDropdown = HelperMethods.FindElement(_driver, Constants.SearchType.XPATH, Constants.PO.XP.REPORTS_LOCATOR);
            SaveDraftButton = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "saveOrderButton");
            SearchBar = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "basicSearchTerm");
            SearchButton = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "basicSearchButton");
            AdvancedSearchLinktext = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "advancedSearchLink");
            GridViewButton = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "gridViewChoice");
            TilesViewButton = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "tileViewChoice");
            MyCartButton = HelperMethods.FindElement(_driver, Constants.SearchType.XPATH, Constants.PO.XP.MYCART_LINK);
            _products = new List<Product>();
            _prodsInCart = new List<Product>();
            #endregion

            if (!_driver.Url.Contains("Products"))
            {
                _logger.Fatal(" > Catalogue product page navigation [FAILED]");
                _logger.Fatal("-- TEST FAILURE @ URL: '" + _driver.Url + "' --");
                BaseDriverTest.TakeScreenshot("screenshot");
            } else
                _logger.Info(" > Products page reached!");
        }

        /// <summary>
        /// Loads all physical elements (locators) of Reports dropdown
        /// </summary>
        public void LoadReportsOptions()
        {
            DLCatAsPDFOption = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "exportCatalogButtonPDF");
            DLCatAsExcelOption = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "exportCatalogButtonXLSX");
            OrderReportOption = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "orderReportButton");
            SummaryOption = HelperMethods.FindElement(_driver, Constants.SearchType.ID, "orderSummaryButton");
        }

        /// <summary>
        /// Loads all products on current catalogue page using row elements
        /// </summary>
        internal void LoadProductRows()
        {
            _logger.Trace(" > Attempting to load catalogue products...");
            Thread.Sleep(2000);

            _productVariants = ProductsTable.FindElements(By.CssSelector(Constants.ProductPage.XP.ALL_PROD_VARIANTS));
            _productQtyUp = ProductsTable.FindElements(By.XPath(Constants.ProductPage.XP.ROW_QTY_UP));
            _productQtyDown = ProductsTable.FindElements(By.XPath(Constants.ProductPage.XP.ROW_QTY_DOWN));
            try
            {
                // apply variant information
                _logger.Trace(" > Building catalogue: variant info...");
                for (int i = 0; i < _productVariants.Count; ++i)
                {
                    string prodInfo = "start" + _productVariants[i].GetAttribute("title") + "end";
                    string varInfo = _productVariants[i].Text + "end";
                    Product newProd = new Product();
                    newProd.ProductNumber = HelperMethods.GetBetween(prodInfo, "Product #: ", " | ");
                    newProd.UPC = HelperMethods.GetBetween(prodInfo, "UPC: ", "end");
                    newProd.Colour = HelperMethods.GetBetween(varInfo, "Color: ", "\r\n");
                    newProd.Size = HelperMethods.GetBetween(varInfo, "Size: ", "\r\n");
                    newProd.Price = HelperMethods.GetBetween(varInfo, "$ ", "end");
                    newProd.QtyUp = _productQtyUp[i];
                    newProd.QtyDown = _productQtyDown[i];
                    newProd.Checked = false;
                    _products.Add(newProd);
                }
                _logger.Info(" > Building catalogue: variant info - Complete!");
            }
            catch (IndexOutOfRangeException) {
                _logger.Error("IndexOutOfRangeException encountered.");
            }

            _productQtyLocators = ProductsTable.FindElements(By.XPath(Constants.ProductPage.XP.PROD_VAR_QTYS));
            try
            {
                // apply quantity box locators
                _logger.Trace(" > Building catalogue: quantity locators...");
                for (int i = 0; i < _productQtyLocators.Count; ++i)
                {
                    _products[i].QtyLocator = _productQtyLocators[i];
                }
                _logger.Info(" > Building catalogue: quantity locators - Complete!");
            }
            catch (IndexOutOfRangeException) {
                _logger.Error("IndexOutOfRangeException encountered.");
            }

            _productRows = ProductsTable.FindElements(By.XPath("//div[contains(@class,'product-row-wrapper')]"));
            _productUpdateBtns = ProductsTable.FindElements(By.XPath("//button[contains(@class, 'btn-update-order')]"));
            int startVal = 0;
            try
            {
                // apply update buttons
                _logger.Trace(" > Building catalogue: update buttons...");
                for (int i = 0; i < _productRows.Count; ++i)
                {
                    int btnCount = _productRows[i].Text.Count(f => f == '$');
                    for (int x = startVal; x < startVal + btnCount; ++x)
                    {
                        _products[x].UpdateButton = _productUpdateBtns[i];
                    }
                    startVal += btnCount;
                }
                _logger.Info(" > Building catalogue: update buttons - Complete!");
            }
            catch (IndexOutOfRangeException) {
                _logger.Error("IndexOutOfRangeException encountered.");
            }
        }

        /// <summary>
        /// Find specific product in catalogue page to interat with
        /// </summary>
        /// <param name="prodNum">name of product to be searched for</param>
        /// <returns>Product object to interact with</returns>
        internal Product LoadProduct(string prodNum)
        {
            _logger.Trace(" > Searching for product: " + prodNum);
            Product currentProd = new Product();
            try
            {
                foreach (Product prod in _products)
                {
                    if (prod.ProductNumber.Equals(prodNum))
                    {
                        currentProd.ProductNumber = prod.ProductNumber;
                        currentProd.UPC = prod.UPC;
                        currentProd.Colour = prod.Colour;
                        currentProd.Size = prod.Size;
                        currentProd.Price = prod.Price;
                        currentProd.QtyUp = prod.QtyUp;
                        currentProd.QtyDown = prod.QtyDown;
                        currentProd.UpdateButton = prod.UpdateButton;
                        currentProd.QtyLocator = prod.QtyLocator;
                        _prodsInCart.Add(currentProd);
                    }
                }
                _logger.Info(" > Product Found!");
            }
            catch (Exception)
            {
                _logger.Fatal(" > Searching for product " +prodNum+ " [FAILED]");
                _logger.Fatal("-- TEST FAILURE @ URL: '" + _driver.Url + "' --");
                BaseDriverTest.TakeScreenshot("screenshot");
            }
            return currentProd;
        }

        /// <summary>
        /// Adds item to cart and checks the resulting alert for verification of addition.
        /// </summary>
        /// <param name="prodNum">name of product to add to cart</param>
        /// <param name="qty">quantity of product to add to cart</param>
        /// <returns>current page object</returns>
        public ProductsPage AddItemToCart(string prodNum, int qty)
        {
            _logger.Trace(" > Attempting to add [" + qty +"] '" +prodNum +"' to cart.");
            WaitForLoad();
            Thread.Sleep(1000);
            Product product = LoadProduct(prodNum);
            _logger.Trace(" > Setting product quantity...");
            product.SetQuantity(qty);
            int qtyValue;
            int.TryParse(product.QtyLocator.GetAttribute("value"), out qtyValue);
            product.Quantity = qtyValue;
            _logger.Trace(" > Adding item to cart...");
            Thread.Sleep(1000);
            product.UpdateButton.Click();
            ItemAdded = HelperMethods.CheckAlert(_driver);
            _logger.Info(" > Item added to cart!");
            return this;
        }

        /// <summary>
        /// Waits for catalogue UI to load.
        /// </summary>
        public ProductsPage WaitForLoad()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(_driver => !_driver.FindElement(By.Id("waitMessage")).Displayed);
            return this;
        }

        public MyCartPage NavigateToCart()
        {
            _logger.Trace(" > Attempting to navigate to cart...");
            try {
                Thread.Sleep(1000);
                MyCartButton.Click();
                Thread.Sleep(2000);
            }
            catch (NoSuchWindowException)
            {
                _logger.Fatal(" > Navigation to cart [FAILED]");
                _logger.Fatal("-- TEST FAILURE @ URL: '" + _driver.Url + "' --");
                BaseDriverTest.TakeScreenshot("screenshot");
            }
            return new MyCartPage(_driver, Constants.OrderType.NEW);
        }

        public ProductsPage SwitchToTileView()
        {
            Thread.Sleep(1500);
            TilesViewButton.Click();
            Thread.Sleep(1000);
            return this;
        }

        /// <summary>
        /// Simulates searching for a product
        /// </summary>
        /// <param name="term">term to search for</param>
        /// <returns>current page object</returns>
        public ProductsPage InputSearchTerm(string term)
        {
            _logger.Info(" > Inputting search term:" + term);
            SearchBar.Clear();
            SearchBar.SendKeys(term);
            return this;
        }

        public ProductsPage InitiateSearch()
        {
            _logger.Trace(" > Searching for item...");
            SearchButton.Click();
            return this;
        }

        public ProductsPage OpenAdvancedSearch()
        {
            _logger.Info(" > Opening Advanced Search");
            AdvancedSearchLinktext.Click();
            return this;
        }

        public ProductsPage SaveDraftOrder()
        {
            SaveDraftButton.Click();
            return this;
        }

        public ProductsPage CataloguePDFExport()
        {
            _logger.Trace(" > Attempting to download catalogue as PDF...");
            ReportsDropdown.Click();
            LoadReportsOptions();
            DLCatAsPDFOption.Click();
            PDFDownloaded = true;
            return this;
        }

        public ProductsPage CatalogueExcelExport()
        {
            ReportsDropdown.Click();
            LoadReportsOptions();
            DLCatAsExcelOption.Click();
            return this;
        }

        public ProductsPage GenerateOrderReport()
        {
            ReportsDropdown.Click();
            LoadReportsOptions();
            OrderReportOption.Click();
            return this;
        }

        public ProductsPage GenerateOrderSummary()
        {
            ReportsDropdown.Click();
            LoadReportsOptions();
            SummaryOption.Click();
            return this;
        }
    }
}
