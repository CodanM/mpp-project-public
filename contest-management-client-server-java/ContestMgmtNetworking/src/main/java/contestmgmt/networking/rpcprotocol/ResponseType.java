package contestmgmt.networking.rpcprotocol;

public enum ResponseType {
    Ok,
    Error,
    NewRegistration,
    GetParticipantById,
    GetCompetitionsByString,
    GetParticipantsByCompetition,
    CountParticipantsForEachCompetition,
    GetParticipantIdsByName,
    GetAgeCategoriesForParticipant,
    AddRegistration
}
