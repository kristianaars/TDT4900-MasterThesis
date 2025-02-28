using CommunityToolkit.Mvvm.Messaging.Messages;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.message;

/// <summary>
/// Message to notify subscribers that the graph of the simulation has changed.
/// </summary>
/// <param name="value">The new graph to run in the simulation</param>
public class NewGraphMessage(Graph value) : ValueChangedMessage<Graph>(value);
