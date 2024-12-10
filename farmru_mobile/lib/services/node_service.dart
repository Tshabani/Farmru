import '../models/NodeAvgDataResponse.dart';
import '../models/nodeDataResponse.dart';
import '../models/nodeResponse.dart';
import '../utils/base_client.dart';

class NodeService {
  static Future<List<NodeResult>?> GetAll() async {
    var response = await BaseClient().get('api/services/app/Node/GetMyNodes');
    if (response == null) return null;
    var node = nodeResponseFromJson(response);

    return node.result;
  }

  static Future<List<NodeData>?> GetNodeDataByNode(NodeResult node) async {
    var response = await BaseClient().get(
        'api/services/app/NodeData/GetNodeDataByNodeId?nodeId=${node.id}&SkipCount=0&MaxResultCount=1000');
    if (response == null) return null;
    var nodeData = nodeDataResponseFromJson(response);

    return nodeData.result.items;
  }

  static Future<NodeAvgData?> GetSensorData() async {
    var response =
        await BaseClient().get('api/services/app/Home/GetSensorData');
    if (response == null) return null;
    var nodeData = nodeAvgDataResponseFromJson(response);

    return nodeData.result;
  }
}
