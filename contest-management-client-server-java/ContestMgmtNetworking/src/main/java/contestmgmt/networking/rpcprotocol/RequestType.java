package contestmgmt.networking.rpcprotocol;

public enum RequestType {
    Login,
    Logout,
    GetParticipantById,
    GetCompetitionsByString,
    GetParticipantsByCompetition,
    CountParticipantsForEachCompetition,
    GetParticipantIdsByName,
    GetAgeCategoriesForParticipant,
    AddRegistration
}
