
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PMJAYBeneficiaryPortalMVC.Models;
using System.Data;

namespace PMJAYBeneficiaryPortalMVC.Controllers;

public class DatabaseSearchController : Controller
{
    private readonly IConfiguration _configuration;

    public DatabaseSearchController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View(new DatabaseSearchViewModel());
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(DatabaseSearchViewModel model)
    {
        model.HasSearched = true;
        model.SearchNumber = model.SearchNumber?.Trim() ?? "";

        model.CMCHISRecords = new();
        model.PMJAYRecords = new();

        if (string.IsNullOrWhiteSpace(model.SearchNumber))
        {
            model.ErrorMessage = "Please enter a Family ID / UFC number.";
            return View(model);
        }

        var cmchisConnection =
            _configuration.GetConnectionString("CMCHISConnection");

        var pmjayConnection =
            _configuration.GetConnectionString("PMJAYConnection");

        if (string.IsNullOrWhiteSpace(cmchisConnection) ||
            string.IsNullOrWhiteSpace(pmjayConnection))
        {
            model.ErrorMessage = "Database connection string is missing.";
            return View(model);
        }

        string currentDatabase = "CMCHIS";

        try
        {
            // 1. CMCHIS SEARCH
            string cmchisSql = @"
            SELECT
                urn,
                membername,
                membername_tamil,
                gender,
                memb_mob,
                ration_card_current,
                address,
                memb_village,
                memb_taluk,
                memb_district,
                ration_shop_code
            FROM dbo.cmchis_data
            WHERE apr_ufc_2026 = @SearchNumber;";

            await using (var connection = new SqlConnection(cmchisConnection))
            {
                await connection.OpenAsync();

                await using var command = new SqlCommand(cmchisSql, connection);
                command.CommandTimeout = 120;

                command.Parameters.Add(
                    "@SearchNumber", SqlDbType.VarChar, 100
                ).Value = model.SearchNumber;

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    model.CMCHISRecords.Add(new CMCHISMatchRow
                    {
                        URN = GetValue(reader, "urn"),
                        MemberName = GetValue(reader, "membername"),
                        TamilName = GetValue(reader, "membername_tamil"),
                        Gender = GetValue(reader, "gender"),
                        Mobile = GetValue(reader, "memb_mob"),
                        RationCard = GetValue(reader, "ration_card_current"),
                        Address = GetValue(reader, "address"),
                        Village = GetValue(reader, "memb_village"),
                        Taluk = GetValue(reader, "memb_taluk"),
                        District = GetValue(reader, "memb_district"),
                        RationShopCode = GetValue(reader, "ration_shop_code")
                    });
                }
            }

            // 2. PMJAY SEARCH
            currentDatabase = "PMJAY";

            string pmjaySql = @"
            SELECT
                enrurn,
                beneficiary_name,
                father_spouse_name,
                gender,
                mobile_number,
                family_id,
                address,
                district_name_cln,
                taluk_name_cln,
                village_name_cln,
                shop_number
            FROM dbo.cmchis_pmjay_secc_full
            WHERE family_id = @SearchNumber;";

            await using (var connection = new SqlConnection(pmjayConnection))
            {
                await connection.OpenAsync();

                await using var command = new SqlCommand(pmjaySql, connection);
                command.CommandTimeout = 120;

                command.Parameters.Add(
                    "@SearchNumber", SqlDbType.VarChar, 100
                ).Value = model.SearchNumber;

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    model.PMJAYRecords.Add(new PMJAYMatchRow
                    {
                        ENR = GetValue(reader, "enrurn"),
                        BeneficiaryName = GetValue(reader, "beneficiary_name"),
                        FatherSpouseName = GetValue(reader, "father_spouse_name"),
                        Gender = GetValue(reader, "gender"),
                        Mobile = GetValue(reader, "mobile_number"),
                        FamilyId = GetValue(reader, "family_id"),
                        Address = GetValue(reader, "address"),
                        District = GetValue(reader, "district_name_cln"),
                        Taluk = GetValue(reader, "taluk_name_cln"),
                        Village = GetValue(reader, "village_name_cln"),
                        ShopNumber = GetValue(reader, "shop_number")
                    });
                }
            }

            model.FirstDatabaseCount = model.CMCHISRecords.Count;
            model.SecondDatabaseCount = model.PMJAYRecords.Count;
        }
        catch (Exception ex)
        {
            model.ErrorMessage =
                currentDatabase + " database error: " + ex.Message;
        }

        return View(model);
    }

    private static string? GetValue(SqlDataReader reader, string column)
    {
        return reader[column] == DBNull.Value
            ? null
            : Convert.ToString(reader[column]);
    }
}
