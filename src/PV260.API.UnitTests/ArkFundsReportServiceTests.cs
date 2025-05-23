using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PV260.API.BL.Options;
using PV260.API.Infrastructure.Services;
using PV260.Common.Models;

namespace PV260.API.UnitTests;

public class ArkFundsReportServiceTests
{
    [Fact]
    public void GenerateNewReport_ShouldReturnReportDetailModel_WhenCsvDataIsValid_WhenNoPreviousReportIsSupplied()
    {
        new Given()
            .ApiReturnsValidCsv()
            .When()
            .ITryToGenerateNewReport()
            .Then()
            .NoExceptionWasThrown()
            .ResultIsNotNull()
            .ResultContainsExpectedRecords();
    }
    
    [Fact]
    public void GenerateNewReport_ShouldReturnReportDetailModel_WhenCsvDataIsValid_WhenPreviousReportIsSupplied()
    {
        new Given()
            .ApiReturnsValidCsv()
            .WithLatestReport()
            .When()
            .ITryToGenerateNewReport()
            .Then()
            .NoExceptionWasThrown()
            .ResultIsNotNull()
            .ResultContainsExpectedRecords();
    }
    
    [Fact]
    public void GenerateNewReport_ShouldThrowException_WhenCsvDataIsCorrupt()
    {
        new Given()
            .ApiReturnsCorruptCsv()
            .When()
            .ITryToGenerateNewReport()
            .Then()
            .ExceptionWasThrown();
    }
    
    [Fact]
    public void GenerateNewReport_ShouldThrowException_WhenApiReturnsError()
    {
        new Given()
            .ApiReturnsError()
            .When()
            .ITryToGenerateNewReport()
            .Then()
            .ExceptionWasThrown();
    }
    
    [Fact]
    public void GenerateNewReport_ShouldThrowException_WhenApiTimeouts()
    {
        new Given()
            .ApiTimeouts()
            .When()
            .ITryToGenerateNewReport()
            .Then()
            .ExceptionWasThrown();
    }
    
    # region Internals

    private class Given
{
    private const string ValidCsvData = "date,fund,company,ticker,cusip,shares,market value ($),weight (%)\n05/23/2025,ARKK,TESLA INC,TSLA,88160R101,\"2,162,086\",\"$737,357,809.44\",13.05%\n05/23/2025,ARKK,COINBASE GLOBAL INC -CLASS A,COIN,19260Q107,\"1,986,860\",\"$540,326,577.00\",9.56%\n05/23/2025,ARKK,ROBLOX CORP -CLASS A,RBLX,771049103,\"5,539,370\",\"$453,175,859.70\",8.02%\n05/23/2025,ARKK,ROKU INC,ROKU,77543R102,\"6,078,264\",\"$424,080,479.28\",7.50%\n05/23/2025,ARKK,TEMPUS AI INC,TEM,88023B103,\"5,107,467\",\"$302,617,419.75\",5.35%\n05/23/2025,ARKK,PALANTIR TECHNOLOGIES INC-A,PLTR,69608A108,\"2,376,524\",\"$290,625,119.96\",5.14%\n05/23/2025,ARKK,ROBINHOOD MARKETS INC - A,HOOD,770700102,\"4,413,661\",\"$285,872,822.97\",5.06%\n05/23/2025,ARKK,SHOPIFY INC - CLASS A,SHOP,82509L107,\"2,638,619\",\"$272,542,956.51\",4.82%\n05/23/2025,ARKK,CRISPR THERAPEUTICS AG,CRSP,H17182108,\"6,741,408\",\"$253,005,042.24\",4.48%\n05/23/2025,ARKK,ARCHER AVIATION INC-A,ACHR,03945R102,\"15,570,277\",\"$165,044,936.20\",2.92%\n05/23/2025,ARKK,AMAZON.COM INC,AMZN,023135106,\"584,113\",\"$118,633,350.30\",2.10%\n05/23/2025,ARKK,BLOCK INC,XYZ,852234103,\"2,008,143\",\"$117,175,144.05\",2.07%\n05/23/2025,ARKK,META PLATFORMS INC-CLASS A,META,30303M102,\"171,853\",\"$109,396,464.21\",1.94%\n05/23/2025,ARKK,ADVANCED MICRO DEVICES,AMD,007903107,\"980,303\",\"$108,529,345.13\",1.92%\n05/23/2025,ARKK,PAGERDUTY INC,PD,69553P100,\"6,664,129\",\"$106,892,629.16\",1.89%\n05/23/2025,ARKK,TWIST BIOSCIENCE CORP,TWST,90184D100,\"3,736,136\",\"$105,209,589.76\",1.86%\n05/23/2025,ARKK,DRAFTKINGS INC-CL A,DKNG UW,26142V105,\"2,814,409\",\"$98,138,441.83\",1.74%\n05/23/2025,ARKK,BEAM THERAPEUTICS INC,BEAM,07373V105,\"5,720,960\",\"$98,629,350.40\",1.74%\n05/23/2025,ARKK,GITLAB INC-CL A,GTLB,37637K108,\"1,934,741\",\"$92,422,577.57\",1.64%\n05/23/2025,ARKK,TERADYNE INC,TER,880770102,\"1,096,728\",\"$86,082,180.72\",1.52%\n05/23/2025,ARKK,NATERA INC,NTRA,632307104,\"536,660\",\"$81,829,916.80\",1.45%\n05/23/2025,ARKK,INTELLIA THERAPEUTICS INC,NTLA,45826J105,\"8,541,878\",\"$80,806,165.88\",1.43%\n05/23/2025,ARKK,TRADE DESK INC/THE -CLASS A,TTD,88339J105,\"1,061,676\",\"$78,914,377.08\",1.40%\n05/23/2025,ARKK,RECURSION PHARMACEUTICALS-A,RXRX,75629V104,\"18,717,808\",\"$77,585,314.16\",1.37%\n05/23/2025,ARKK,10X GENOMICS INC-CLASS A,TXG,88025U109,\"8,894,589\",\"$75,959,790.06\",1.34%\n05/23/2025,ARKK,PINTEREST INC- CLASS A,PINS,72352L106,\"2,225,143\",\"$70,648,290.25\",1.25%\n05/23/2025,ARKK,ILLUMINA INC,ILMN,452327109,\"769,796\",\"$61,999,369.84\",1.10%\n05/23/2025,ARKK,DEERE & CO,DE,244199105,\"117,727\",\"$60,705,927.55\",1.07%\n05/23/2025,ARKK,IRIDIUM COMMUNICATIONS INC,IRDM,46269C102,\"2,274,469\",\"$58,067,193.57\",1.03%\n05/23/2025,ARKK,VERACYTE INC,VCYT,92337F107,\"1,900,460\",\"$52,072,604.00\",0.92%\n05/23/2025,ARKK,NVIDIA CORP,NVDA,67066G104,\"382,090\",\"$50,753,014.70\",0.90%\n05/23/2025,ARKK,TAIWAN SEMICONDUCTOR-SP ADR,TSM,874039100,\"149,691\",\"$29,367,877.29\",0.52%\n05/23/2025,ARKK,GUARDANT HEALTH INC,GH,40131M109,\"776,110\",\"$28,754,875.50\",0.51%\n05/23/2025,ARKK,AIRBNB INC-CLASS A,ABNB,009066101,\"214,945\",\"$27,437,729.25\",0.49%\n05/23/2025,ARKK,PACIFIC BIOSCIENCES OF CALIF,PACB,69404D108,\"22,471,812\",\"$21,157,211.00\",0.37%\n05/23/2025,ARKK,GOLDMAN FS TRSY OBLIG INST 468,,X9USDGSFT,\"15,385,671\",\"$15,385,670.98\",0.27%\n05/23/2025,ARKK,CERUS CORP,CERS,157085101,\"10,931,351\",\"$13,664,188.75\",0.24%\n05/23/2025,ARKK,SOFI TECHNOLOGIES INC,SOFI,83406F102,\"109,169\",\"$1,439,939.11\",0.03%\n\"Investors should carefully consider the investment objectives and risks as well as charges and expenses of an ARK ETF before investing. This and other information are contained in the ARK ETFs' prospectuses, which may be obtained on ark-funds.com. The prospectus should be read carefully before investing. An investment in an ARK ETF is subject to risks and you can lose money on your investment in an ARK ETF. There can be no assurance that the ARK ETFs will achieve their investment objectives. The ARK ETFs' portfolios are more volatile than broad market averages. The ARK ETFs also have specific risks, which are described in the ARK ETFs' prospectuses.Shares of the ARK ETFs may be bought or sold throughout the day at their market price on the exchange on which they are listed. The market price of an ARK ETF's shares may be at, above or below the ARK ETF's net asset value (\"\"NAV\"\") and will fluctuate with changes in the NAV as well as supply and demand in the market for the shares. The market price of ARK ETF shares may differ significantly from their NAV during periods of market volatility. Shares of the ARK ETFs may only be redeemed directly with the ARK ETFs at NAV by Authorized Participants, in very large creation units. There can be no guarantee that an active trading market for ARK ETF shares will develop or be maintained, or that their listing will continue or remain unchanged. Buying or selling ARK ETF shares on an exchange may require the payment of brokerage commissions and frequent trading may incur brokerage costs that detract significantly from investment returns. Not FDIC Insured – No Bank Guarantee – May Lose ValueAll statements made regarding companies, securities or other financial information are strictly beliefs and points of view held by ARK Investment Management LLC and/or ARK ETF Trust and are not endorsements by ARK of any company or security or recommendations by ARK to buy, sell or hold any security. Holdings are subject to change without notice.Foreside Fund Services, LLC, distributor.© 2024. ARK ETF Trust. No part of this material may be reproduced in any form, or referred to in any other publication, without written permission.\"";
    private const string CorruptCsvData = "corrupt csv data";

    private readonly Mock<HttpMessageHandler> _handlerMock = new();
    private readonly IOptions<ReportOptions> _reportOptions = new OptionsWrapper<ReportOptions>(new ReportOptions
    {
        ArkFundsCsvUrl = "https://example.com/ark_funds.csv",
        OldReportCleanupCron = "0 0 * * 0",
        ReportDaysToKeep = 30,
        ReportGenerationCron = "0 0 * * *",
        SendEmailOnReportGeneration = false
    });
    private readonly ReportDetailModel _latestReport = JsonConvert.DeserializeObject<ReportDetailModel>("{\n  \"name\": \"ARK Innovation ETF Report - 2025-05-22\",\n  \"createdAt\": \"2025-05-22T22:59:00.6956479\",\n  \"records\": [\n    {\n      \"companyName\": \"AIRBNB INC-CLASS A\",\n      \"ticker\": \"ABNB\",\n      \"numberOfShares\": 211521,\n      \"sharesChangePercentage\": -0.503313874999412,\n      \"weight\": 0.49,\n      \"id\": \"fdc6d055-1756-4cbf-84d9-047bc672db3a\"\n    },\n    {\n      \"companyName\": \"RECURSION PHARMACEUTICALS-A\",\n      \"ticker\": \"RXRX\",\n      \"numberOfShares\": 18419216,\n      \"sharesChangePercentage\": -0.5040371043908325,\n      \"weight\": 1.37,\n      \"id\": \"46558ad0-0592-440f-a9f6-051c49e40cfb\"\n    },\n    {\n      \"companyName\": \"NATERA INC\",\n      \"ticker\": \"NTRA\",\n      \"numberOfShares\": 528116,\n      \"sharesChangePercentage\": -0.5030275855052695,\n      \"weight\": 1.45,\n      \"id\": \"165c705e-1ec2-41f9-a7a5-089df108f0da\"\n    },\n    {\n      \"companyName\": \"10X GENOMICS INC-CLASS A\",\n      \"ticker\": \"TXG\",\n      \"numberOfShares\": 8752701,\n      \"sharesChangePercentage\": -0.5040331174993955,\n      \"weight\": 1.32,\n      \"id\": \"5cce919a-f4b4-4209-b676-0977f4a01a0e\"\n    },\n    {\n      \"companyName\": \"TAIWAN SEMICONDUCTOR-SP ADR\",\n      \"ticker\": \"TSM\",\n      \"numberOfShares\": 147323,\n      \"sharesChangePercentage\": -0.4997872527235029,\n      \"weight\": 0.51,\n      \"id\": \"0d98500c-d029-4733-ae77-0c6cde89f7e3\"\n    },\n    {\n      \"companyName\": \"PACIFIC BIOSCIENCES OF CALIF\",\n      \"ticker\": \"PACB\",\n      \"numberOfShares\": 22113252,\n      \"sharesChangePercentage\": -0.5040208281815404,\n      \"weight\": 0.39,\n      \"id\": \"4233c6a0-0b20-4105-9550-10670017506d\"\n    },\n    {\n      \"companyName\": \"DEERE & CO\",\n      \"ticker\": \"DE\",\n      \"numberOfShares\": 115839,\n      \"sharesChangePercentage\": -0.5067466009327573,\n      \"weight\": 1.08,\n      \"id\": \"7b673117-d490-4591-8fbb-1537620cca69\"\n    },\n    {\n      \"companyName\": \"VERACYTE INC\",\n      \"ticker\": \"VCYT\",\n      \"numberOfShares\": 1870156,\n      \"sharesChangePercentage\": -0.5038236329993308,\n      \"weight\": 0.94,\n      \"id\": \"a3f063eb-5fda-4eb8-8d83-21aabb03cd9d\"\n    },\n    {\n      \"companyName\": \"COINBASE GLOBAL INC -CLASS A\",\n      \"ticker\": \"COIN\",\n      \"numberOfShares\": 1955180,\n      \"sharesChangePercentage\": -0.503796283102978,\n      \"weight\": 9.21,\n      \"id\": \"f0c711cf-a818-4e99-b6cf-28ef7b59e8e5\"\n    },\n    {\n      \"companyName\": \"ROBINHOOD MARKETS INC - A\",\n      \"ticker\": \"HOOD\",\n      \"numberOfShares\": 4418137,\n      \"sharesChangePercentage\": -0.6047220966758957,\n      \"weight\": 5.13,\n      \"id\": \"27cbf86a-1ff2-4d79-9c1b-2e5dc0539959\"\n    },\n    {\n      \"companyName\": \"TESLA INC\",\n      \"ticker\": \"TSLA\",\n      \"numberOfShares\": 2127590,\n      \"sharesChangePercentage\": -0.5041222987602707,\n      \"weight\": 12.95,\n      \"id\": \"17cdedad-9180-42e4-8cef-2fd107fe4d56\"\n    },\n    {\n      \"companyName\": \"NVIDIA CORP\",\n      \"ticker\": \"NVDA\",\n      \"numberOfShares\": 376010,\n      \"sharesChangePercentage\": -0.5027652086475616,\n      \"weight\": 0.9,\n      \"id\": \"8a264adf-e121-4b21-9ae2-3b927a9813e8\"\n    },\n    {\n      \"companyName\": \"SHOPIFY INC - CLASS A\",\n      \"ticker\": \"SHOP\",\n      \"numberOfShares\": 2596539,\n      \"sharesChangePercentage\": -0.5038914598636083,\n      \"weight\": 4.83,\n      \"id\": \"726eb932-fb7f-4bd6-886d-3c0e87920a2d\"\n    },\n    {\n      \"companyName\": \"CRISPR THERAPEUTICS AG\",\n      \"ticker\": \"CRSP\",\n      \"numberOfShares\": 6633856,\n      \"sharesChangePercentage\": -0.5040895596617965,\n      \"weight\": 4.46,\n      \"id\": \"c33e26e7-40f7-455d-8e44-3e890e04b000\"\n    },\n    {\n      \"companyName\": \"IRIDIUM COMMUNICATIONS INC\",\n      \"ticker\": \"IRDM\",\n      \"numberOfShares\": 2238181,\n      \"sharesChangePercentage\": -0.5041073188469901,\n      \"weight\": 1.03,\n      \"id\": \"49b14856-ee7b-4780-afbd-4d9ea62abdc7\"\n    },\n    {\n      \"companyName\": \"GUARDANT HEALTH INC\",\n      \"ticker\": \"GH\",\n      \"numberOfShares\": 763726,\n      \"sharesChangePercentage\": -0.5041714651978385,\n      \"weight\": 0.52,\n      \"id\": \"2fb218e6-6b08-49ff-ba0c-4eca6791a617\"\n    },\n    {\n      \"companyName\": \"SOFI TECHNOLOGIES INC\",\n      \"ticker\": \"SOFI\",\n      \"numberOfShares\": 107409,\n      \"sharesChangePercentage\": -0.5094526625848702,\n      \"weight\": 0.03,\n      \"id\": \"82dc0b05-898a-450e-92d5-52b97dbeb03e\"\n    },\n    {\n      \"companyName\": \"INTELLIA THERAPEUTICS INC\",\n      \"ticker\": \"NTLA\",\n      \"numberOfShares\": 8405622,\n      \"sharesChangePercentage\": -0.5040125697752019,\n      \"weight\": 1.41,\n      \"id\": \"7752a233-246f-404d-b35c-5b416a097ff7\"\n    },\n    {\n      \"companyName\": \"ROKU INC\",\n      \"ticker\": \"ROKU\",\n      \"numberOfShares\": 5981304,\n      \"sharesChangePercentage\": -0.5040252152337379,\n      \"weight\": 7.57,\n      \"id\": \"e7ae6ffe-3376-4afd-bbf8-651b3329a097\"\n    },\n    {\n      \"companyName\": \"TEMPUS AI INC\",\n      \"ticker\": \"TEM\",\n      \"numberOfShares\": 5025995,\n      \"sharesChangePercentage\": -0.5040132001571824,\n      \"weight\": 5.32,\n      \"id\": \"baec3990-9180-4539-abd8-6bcbfe0c81ba\"\n    },\n    {\n      \"companyName\": \"ARCHER AVIATION INC-A\",\n      \"ticker\": \"ACHR\",\n      \"numberOfShares\": 15321893,\n      \"sharesChangePercentage\": -0.5040419135332397,\n      \"weight\": 3.02,\n      \"id\": \"6ce1aa20-4fef-476b-983f-6da1940cd71c\"\n    },\n    {\n      \"companyName\": \"META PLATFORMS INC-CLASS A\",\n      \"ticker\": \"META\",\n      \"numberOfShares\": 169101,\n      \"sharesChangePercentage\": -0.5059984349350733,\n      \"weight\": 1.96,\n      \"id\": \"99ab5aa2-8c97-4c21-a935-7b47b1a10fd3\"\n    },\n    {\n      \"companyName\": \"ILLUMINA INC\",\n      \"ticker\": \"ILMN\",\n      \"numberOfShares\": 757508,\n      \"sharesChangePercentage\": -0.5043685673305768,\n      \"weight\": 1.09,\n      \"id\": \"2456fd26-abaa-46b9-b9f3-807a7d4ea545\"\n    },\n    {\n      \"companyName\": \"GITLAB INC-CL A\",\n      \"ticker\": \"GTLB\",\n      \"numberOfShares\": 1903861,\n      \"sharesChangePercentage\": -0.5043085720437458,\n      \"weight\": 1.65,\n      \"id\": \"d8b0f511-1b2c-42af-86ef-8c858ec5d1d9\"\n    },\n    {\n      \"companyName\": \"BEAM THERAPEUTICS INC\",\n      \"ticker\": \"BEAM\",\n      \"numberOfShares\": 5629696,\n      \"sharesChangePercentage\": -0.5040457981809107,\n      \"weight\": 1.76,\n      \"id\": \"92514847-9703-4f59-b7a0-9fdc28b1b9d0\"\n    },\n    {\n      \"companyName\": \"AMAZON.COM INC\",\n      \"ticker\": \"AMZN\",\n      \"numberOfShares\": 574801,\n      \"sharesChangePercentage\": -0.5037120636442789,\n      \"weight\": 2.1,\n      \"id\": \"a16fad8e-2858-4b18-a094-a12ac8bd3f55\"\n    },\n    {\n      \"companyName\": \"TRADE DESK INC/THE -CLASS A\",\n      \"ticker\": \"TTD\",\n      \"numberOfShares\": 1044748,\n      \"sharesChangePercentage\": -0.5037912913627888,\n      \"weight\": 1.42,\n      \"id\": \"966e7591-5aaf-4efa-8633-ac1462a64518\"\n    },\n    {\n      \"companyName\": \"CERUS CORP\",\n      \"ticker\": \"CERS\",\n      \"numberOfShares\": 10756919,\n      \"sharesChangePercentage\": -0.5040046121647974,\n      \"weight\": 0.24,\n      \"id\": \"7f3006b4-42e2-4fab-b377-b02250a4c718\"\n    },\n    {\n      \"companyName\": \"TERADYNE INC\",\n      \"ticker\": \"TER\",\n      \"numberOfShares\": 1079224,\n      \"sharesChangePercentage\": -0.5042896890736005,\n      \"weight\": 1.55,\n      \"id\": \"ee1532cc-7336-4955-ae27-bb51f2c61c31\"\n    },\n    {\n      \"companyName\": \"PALANTIR TECHNOLOGIES INC-A\",\n      \"ticker\": \"PLTR\",\n      \"numberOfShares\": 2338604,\n      \"sharesChangePercentage\": -0.5041579201294728,\n      \"weight\": 5.13,\n      \"id\": \"877ef0e8-c62d-4251-adde-c7cba3814d28\"\n    },\n    {\n      \"companyName\": \"BLOCK INC\",\n      \"ticker\": \"XYZ\",\n      \"numberOfShares\": 1976111,\n      \"sharesChangePercentage\": -0.503997490585921,\n      \"weight\": 1.99,\n      \"id\": \"b8940f9d-e205-4c89-8fe8-ca3fd8968ad4\"\n    },\n    {\n      \"companyName\": \"GOLDMAN FS TRSY OBLIG INST 468\",\n      \"ticker\": \"\",\n      \"numberOfShares\": 15171447,\n      \"sharesChangePercentage\": -21.40451805801161,\n      \"weight\": 0.28,\n      \"id\": \"ff7e679f-db4e-4c0c-85db-ca69507de53d\"\n    },\n    {\n      \"companyName\": \"PINTEREST INC- CLASS A\",\n      \"ticker\": \"PINS\",\n      \"numberOfShares\": 2189655,\n      \"sharesChangePercentage\": -0.5039202633653604,\n      \"weight\": 1.28,\n      \"id\": \"cd08b23b-546a-48ae-969e-cbf331b2a326\"\n    },\n    {\n      \"companyName\": \"ROBLOX CORP -CLASS A\",\n      \"ticker\": \"RBLX\",\n      \"numberOfShares\": 5451018,\n      \"sharesChangePercentage\": -0.5039582902872763,\n      \"weight\": 8.05,\n      \"id\": \"6c57c38f-2408-4b8c-9c03-deadf8c5e2c3\"\n    },\n    {\n      \"companyName\": \"PAGERDUTY INC\",\n      \"ticker\": \"PD\",\n      \"numberOfShares\": 6557825,\n      \"sharesChangePercentage\": -0.5040171930247783,\n      \"weight\": 1.88,\n      \"id\": \"ce8328ea-9081-4061-848c-e51c8daf569f\"\n    },\n    {\n      \"companyName\": \"ADVANCED MICRO DEVICES\",\n      \"ticker\": \"AMD\",\n      \"numberOfShares\": 964655,\n      \"sharesChangePercentage\": -0.5043602927146239,\n      \"weight\": 1.97,\n      \"id\": \"ffa3221f-8f32-4d7c-850c-eeee25a88e99\"\n    },\n    {\n      \"companyName\": \"DRAFTKINGS INC-CL A\",\n      \"ticker\": \"DKNG UW\",\n      \"numberOfShares\": 2769513,\n      \"sharesChangePercentage\": -0.5040338877466596,\n      \"weight\": 1.79,\n      \"id\": \"bb1596be-962c-4b3f-919a-ff1b55aa8641\"\n    },\n    {\n      \"companyName\": \"TWIST BIOSCIENCE CORP\",\n      \"ticker\": \"TWST\",\n      \"numberOfShares\": 3676520,\n      \"sharesChangePercentage\": -0.5041743907554497,\n      \"weight\": 1.9,\n      \"id\": \"649b8367-3605-49d4-be82-fff88796e9d7\"\n    }\n  ],\n  \"id\": \"d4510fb6-146e-4919-a556-1a980766401b\"\n}")!;

    private bool _includeLatestReport;
    private HttpClient? _httpClient;

    public Given ApiReturnsValidCsv()
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(ValidCsvData)
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return this;
    }

    public Given ApiReturnsCorruptCsv()
    {
        var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        {
            Content = new StringContent(CorruptCsvData)
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        return this;
    }

    public Given ApiReturnsError()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("API error"));

        return this;
    }

    public Given ApiTimeouts()
    {
        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("API timeout"));

        return this;
    }

    public Given WithLatestReport()
    {
        _includeLatestReport = true;
        return this;
    }

    public When When()
    {
        _httpClient = new HttpClient(_handlerMock.Object);
        var reportService = new ArkFundsReportService(_httpClient, _reportOptions, new NullLogger<ArkFundsReportService>());
        return new When(reportService, _includeLatestReport ? _latestReport : null);
    }
}

    private class When(ArkFundsReportService reportService, ReportDetailModel? latestReport)
    {
        private ReportDetailModel? _result;
        private Exception? _exception;

        public When ITryToGenerateNewReport()
        {
            try
            {
                _result = reportService.GenerateNewReportAsync(latestReport).Result;
            }
            catch (Exception ex)
            {
                _exception = ex;
            }

            return this;
        }
        
        public Then Then()
        {
            return new Then(_result, _exception, latestReport != null);
        }
    }

    private class Then(ReportDetailModel? result, Exception? exception, bool latestReportIncluded)
    {
        public Then NoExceptionWasThrown()
        {
            Assert.Null(exception);
            return this;
        }

        public Then ExceptionWasThrown()
        {
            Assert.NotNull(exception);
            return this;
        }

        public Then ResultIsNotNull()
        {
            Assert.NotNull(result);
            return this;
        }

        public Then ResultIsNull()
        {
            Assert.Null(result);
            return this;
        }
        
        public Then ResultContainsExpectedRecords()
        {
            if (latestReportIncluded)
            {
                Assert.Equal(38, result?.Records.Count);
                Assert.Contains(result?.Records!,
                    r => r is { Ticker: "TSLA", NumberOfShares: 2162086, Weight: 13.05 } &&
                         Math.Abs(r.SharesChangePercentage - 1.62) < 0.01);
            }
            else
            {
                Assert.Equal(37, result?.Records.Count);
                Assert.Contains(result?.Records!,
                    r => r is { Ticker: "TSLA", NumberOfShares: 2162086, SharesChangePercentage: 0.0, Weight: 13.05 });
            }

            return this;
        }
    }
    
    #endregion
}