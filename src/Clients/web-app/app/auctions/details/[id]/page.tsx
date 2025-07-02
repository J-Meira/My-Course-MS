import { Heading } from "@/components";
import { BidList, DeleteButton, SpecsTable, UpdateButton } from "./components";
import { CarImage, CountdownTimer } from "../../components";

import { getAuctionById, getCurrentUser } from "@/services";

export default async function Details({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = await params;
  const data = await getAuctionById(id);
  const user = await getCurrentUser();

  return (
    <div>
      <div className="flex justify-between">
        <div className="flex items-center gap-3">
          <Heading title={`${data.make} ${data.model}`} />
          {user?.username === data.seller && (
            <>
              <UpdateButton id={data.id} />
              <DeleteButton id={data.id} />
            </>
          )}
        </div>

        <div className="flex gap-3">
          <h3 className="text-2xl font-semibold">Time remaining:</h3>
          <CountdownTimer auctionEnd={data.auctionEnd} />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-6 mt-3">
        <div className="relative bg-gray-200 aspect-video rounded-lg overflow-hidden">
          <CarImage imageUrl={data.imageUrl} />
        </div>

        <BidList user={user} auction={data} />
      </div>

      <div className="mt-3 grid grid-cols-1 rounded-lg">
        <SpecsTable auction={data} />
      </div>
    </div>
  );
}
