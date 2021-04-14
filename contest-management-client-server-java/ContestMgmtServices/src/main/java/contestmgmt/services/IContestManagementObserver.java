package contestmgmt.services;

import contestmgmt.model.Registration;

public interface IContestManagementObserver {
    void newRegistration(Registration r) throws ContestManagementException;
}
