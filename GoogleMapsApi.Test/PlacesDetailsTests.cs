﻿using System;
using System.Linq;
using GoogleMapsApi.Entities.Common;
using GoogleMapsApi.Entities.PlacesDetails.Request;
using GoogleMapsApi.Entities.PlacesDetails.Response;
using NUnit.Framework;

namespace GoogleMapsApi.Test
{
    [TestFixture]
    public class PlacesDetailsTests
    {
        public string ApiKey = ""; // your API key goes here...

        [Test]
        public void ReturnsStronglyTypedPriceLevel()
        {
            if (ApiKey == "") Assert.Inconclusive("API key not specified");
            var request = new PlacesDetailsRequest
            {
                ApiKey = ApiKey,
                Reference = GetMyPlaceReference(),
            };

            PlacesDetailsResponse result = GoogleMaps.PlacesDetails.Query(request);

            if (result.Status == Status.OVER_QUERY_LIMIT)
                Assert.Inconclusive("Cannot run test since you have exceeded your Google API query limit.");
            Assert.AreEqual(Status.OK, result.Status);
            Assert.AreEqual(PriceLevel.Moderate, result.Result.PriceLevel);
        }

        [Test]
        public void ReturnsOpeningTimes()
        {
            if (ApiKey == "") Assert.Inconclusive("API key not specified");
            var request = new PlacesDetailsRequest
            {
                ApiKey = ApiKey,
                Reference = GetMyPlaceReference(),
            };

            PlacesDetailsResponse result = GoogleMaps.PlacesDetails.Query(request);

            if (result.Status == Status.OVER_QUERY_LIMIT)
                Assert.Inconclusive("Cannot run test since you have exceeded your Google API query limit.");
            Assert.AreEqual(Status.OK, result.Status);
            Assert.AreEqual(7, result.Result.OpeningHours.Periods.Count());
            var sundayPeriod = result.Result.OpeningHours.Periods.First();
            Assert.That(sundayPeriod.OpenTime.Day, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(sundayPeriod.OpenTime.Time, Is.GreaterThanOrEqualTo(0));
            Assert.That(sundayPeriod.OpenTime.Time, Is.LessThanOrEqualTo(2359));
            Assert.That(sundayPeriod.CloseTime.Time, Is.GreaterThanOrEqualTo(0));
            Assert.That(sundayPeriod.CloseTime.Time, Is.LessThanOrEqualTo(2359));
        }

        private string cachedMyPlaceReference;
        private string GetMyPlaceReference()
        {
            if (cachedMyPlaceReference == null)
            {
                var referenceRequest = new Entities.Places.Request.PlacesRequest()
                {
                    ApiKey = ApiKey,
                    Name = "My Place Bar & Restaurant",
                    Location = new Location(-31.954453, 115.862717),
                    RankBy = Entities.Places.Request.RankBy.Distance,
                };
                var referenceResult = GoogleMaps.Places.Query(referenceRequest);
                if (referenceResult.Status == Entities.Places.Response.Status.OVER_QUERY_LIMIT)
                    Assert.Inconclusive("Cannot run test since you have exceeded your Google API query limit.");
                cachedMyPlaceReference = referenceResult.Results.First().Reference;
            }
            return cachedMyPlaceReference;
        }
    }
}