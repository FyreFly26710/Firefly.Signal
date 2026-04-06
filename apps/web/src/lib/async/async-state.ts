export type AsyncStatus = "idle" | "loading" | "success" | "error";

export type AsyncState<TData, TStatus extends string = AsyncStatus> = {
  status: TStatus;
  data: TData | null;
  errorMessage: string | null;
};

export function createAsyncState<TData, TStatus extends string = AsyncStatus>(
  status: TStatus,
  data: TData | null = null,
  errorMessage: string | null = null
): AsyncState<TData, TStatus> {
  return {
    status,
    data,
    errorMessage
  };
}
