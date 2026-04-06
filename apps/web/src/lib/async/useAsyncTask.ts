import { useCallback, useRef, useState } from "react";
import { createAsyncState, type AsyncState } from "@/lib/async/async-state";

export function useAsyncTask<TArgs extends unknown[], TData>(task: (...args: TArgs) => Promise<TData>) {
  const [state, setState] = useState<AsyncState<TData>>(createAsyncState<TData>("idle"));
  const latestRequestIdRef = useRef(0);

  const execute = useCallback(
    async (...args: TArgs): Promise<TData | null> => {
      const requestId = latestRequestIdRef.current + 1;
      latestRequestIdRef.current = requestId;
      setState(createAsyncState<TData>("loading"));

      try {
        const data = await task(...args);

        if (requestId !== latestRequestIdRef.current) {
          return null;
        }

        setState(createAsyncState<TData>("success", data));
        return data;
      } catch (error: unknown) {
        if (requestId !== latestRequestIdRef.current) {
          return null;
        }

        const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
        setState(createAsyncState<TData>("error", null, errorMessage));
        return null;
      }
    },
    [task]
  );

  return {
    ...state,
    execute
  };
}
