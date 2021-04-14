package contestmgmt.networking.utils;

import contestmgmt.networking.rpcprotocol.ContestMgmtClientRpcReflectionWorker;
import contestmgmt.services.IContestManagementServices;

import java.net.Socket;
import java.rmi.ServerException;

public class ContestMgmtRpcConcurrentServer extends AbstractConcurrentServer {
    private IContestManagementServices contestMgmtServer;

    public ContestMgmtRpcConcurrentServer(int port, IContestManagementServices contestMgmtServer) {
        super(port);
        this.contestMgmtServer = contestMgmtServer;
        System.out.println("ContestMgmt - ContestMgmtRpcConcurrentServer");
    }

    @Override
    protected Thread createWorker(Socket client) {
        ContestMgmtClientRpcReflectionWorker worker = new ContestMgmtClientRpcReflectionWorker(contestMgmtServer, client);
        return new Thread(worker);
    }

    @Override
    public void stop() throws ServerException {
        super.stop();
        System.out.println("Stopping services...");
    }
}
