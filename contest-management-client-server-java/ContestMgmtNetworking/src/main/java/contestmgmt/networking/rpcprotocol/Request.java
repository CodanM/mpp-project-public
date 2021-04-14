package contestmgmt.networking.rpcprotocol;

import java.io.Serializable;

public class Request implements Serializable {
    private RequestType type;

    private Object data;

    public RequestType getType() {
        return type;
    }

    private void setType(RequestType type) {
        this.type = type;
    }

    public Object getData() {
        return data;
    }

    private void setData(Object data) {
        this.data = data;
    }

    @Override
    public String toString() {
        return "Request { type='%s', data='%s' }".formatted(type.toString(), data == null ? "null" : data.toString());
    }

    public static class Builder {
        private Request request = new Request();

        public Builder type(RequestType type) {
            request.setType(type);
            return this;
        }

        public Builder data(Object data) {
            request.setData(data);
            return this;
        }

        public Request build() {
            return request;
        }
    }
}
