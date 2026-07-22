using Microsoft.AspNetCore.Mvc.Filters;
using Pds.Core.Web.Models.Hyperlinks;
using Pds.DocumentExchange.Web.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pds.DocumentExchange.Web.Attributes.BreadCrumbs
{
    /// <summary>
    /// Base class for a breadcrumb attribute.
    /// </summary>
    public abstract class BreadCrumbAttribute : Attribute, IFilterMetadata
    {
        /// <summary>
        /// Gets the parent breadcrumb attribute.
        /// </summary>
        /// <param name="breadCrumbData">The bread crumb data.</param>
        /// <returns>The <see cref="BreadCrumbAttribute"/> representing the parent breadcrumb.</returns>
        public abstract BreadCrumbAttribute GetParent(BreadCrumbData breadCrumbData);

        /// <summary>
        /// Gets the view model representing this breadcrumb.
        /// </summary>
        public abstract BreadCrumbViewModel ViewModel { get; }

        /// <summary>
        /// Gets the list of breadcrumbs representing the path to this breadcrumb inclusive.
        /// </summary>
        /// <param name="breadCrumbData">The bread crumb data.</param>
        /// <returns>The list of breadcrumbs representing the path to this breadcrumb inclusive.</returns>
        public IList<BreadCrumbViewModel> GetPath(BreadCrumbData breadCrumbData)
        {
            // Using a Stack here (last in / first out), because we are starting at the final breadcrumb and
            // therefore, at the end, we want to output the list of breadcrumbs in reverse order.
            var breadCrumbs = new Stack<BreadCrumbViewModel>();
            var currentBreadCrumb = this;

            do
            {
                breadCrumbs.Push(currentBreadCrumb.ViewModel);
                currentBreadCrumb = currentBreadCrumb.GetParent(breadCrumbData);
            }
            while (currentBreadCrumb != null);

            var breadCrumbsList = breadCrumbs.ToList();
            return breadCrumbsList.Count > 1
                ? breadCrumbsList
                : Enumerable.Empty<BreadCrumbViewModel>().ToList();
        }

        /// <summary>
        /// The text to display for the home page bread crumb.
        /// </summary>
        protected const string HomeBreadCrumbText = "Home";
    }
}