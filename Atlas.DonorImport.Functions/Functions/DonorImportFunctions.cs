using System.IO;
using System.Threading.Tasks;
using Atlas.DonorImport.ExternalInterface.Models;
using Atlas.DonorImport.Services;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;

namespace Atlas.DonorImport.Functions.Functions
{
    public class DonorImportFunctions
    {
        private readonly IDonorFileImporter donorFileImporter;

        public DonorImportFunctions(IDonorFileImporter donorFileImporter)
        {
            this.donorFileImporter = donorFileImporter;
        }

        /// IMPORTANT: Do not rename this function without careful consideration. This function is called by event grid, which has the function name set by terraform.
        // If changing this, you must also change the value hardcoded in the event_grid.tf terraform file. 
        [FunctionName(nameof(ImportDonorFile))]
        public async Task ImportDonorFile(
            // Raw JSON Text file containing donor updates in expected schema
            [EventGridTrigger] EventGridEvent blobCreatedEvent,
            [Blob("{data.url}", FileAccess.Read)] Stream blobStream)
        {
            await donorFileImporter.ImportDonorFile(new DonorImportFile {Contents = blobStream, FileName = "TODO:ATLAS-376"});
        }
    }
}