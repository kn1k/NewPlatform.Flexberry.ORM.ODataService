﻿namespace NewPlatform.Flexberry.ORM.ODataService.Tests.Handlers
{
    using System.Reflection;
    using System.Web.OData.Routing;
    using System.Web.OData.Routing.Conventions;

    using NewPlatform.Flexberry.ORM.ODataService.Handlers;
    using NewPlatform.Flexberry.ORM.ODataService.Model;

    using Xunit;

    /// <summary>
    /// Unit test class for <see cref="PerRequestUpdateEdmModelHandler"/>.
    /// </summary>

    public class PerRequestUpdateEdmModelHandlerTest
    {
        /// <summary>
        /// Tests the <see cref="PerRequestUpdateEdmModelHandler"/> constructor.
        /// It will throw an exception if OData API changes.
        /// </summary>
        [Fact]
        public void TestWebApi()
        {
            var pathHandler = new DefaultODataPathHandler();
            var model = new DataObjectEdmModel(new DataObjectEdmMetadata());
            var conventions = new IODataRoutingConvention[0];
            //-solo- var constraint = new ODataPathRouteConstraint(pathHandler, model, "name", conventions);
            var constraint = new ODataPathRouteConstraint("name");
            throw new System.NotImplementedException("-solo-");
            var route = new ODataRoute("prefix", constraint);
            var assemblies = new Assembly[0];
            var modelBuilder = new DefaultDataObjectEdmModelBuilder(assemblies);

            new PerRequestUpdateEdmModelHandler(new ManagementToken(route, model), modelBuilder);
        }
    }
}
