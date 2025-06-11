"use client";

import { useState } from "react";
import { Button, Spinner } from "flowbite-react";
import { updateAuctionTest } from "@/services";

export const AuthTest = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [result, setResult] = useState<any>();

  function doUpdate() {
    setResult(undefined);
    setIsLoading(true);
    updateAuctionTest()
      .then((res) => setResult(res))
      .finally(() => setIsLoading(false));
  }

  return (
    <div className="flex items-center gap-4">
      <Button outline onClick={doUpdate}>
        {isLoading && <Spinner size="sm" />}
        Test auth
      </Button>
      <div>{JSON.stringify(result, null, 2)}</div>
    </div>
  );
};
