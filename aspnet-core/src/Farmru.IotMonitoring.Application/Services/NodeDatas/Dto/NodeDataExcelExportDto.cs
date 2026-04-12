namespace Farmru.IotMonitoring.Services.NodeData.Dto
{
    /// <summary>
    /// Excel file payload returned from node data export. Serialized as JSON; file bytes are base64-encoded.
    /// </summary>
    public class NodeDataExcelExportDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
