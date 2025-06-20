"use client";

import { Button, Spinner } from "flowbite-react";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { toast } from "react-hot-toast";

import { deleteAuction } from "@/services";

type Props = {
  id: string;
};

export const DeleteButton = ({ id }: Props) => {
  const [isLoading, setIsLoading] = useState(false);
  const router = useRouter();

  function doDelete() {
    setIsLoading(true);
    deleteAuction(id)
      .then((res) => {
        if (res.error) throw res.error;
        router.push("/");
      })
      .catch((error) => {
        toast.error(error.status + " " + error.message);
      })
      .finally(() => setIsLoading(false));
  }

  return (
    <Button color="failure" onClick={doDelete}>
      {isLoading && <Spinner size="sm" />}
      Delete Auction
    </Button>
  );
};
