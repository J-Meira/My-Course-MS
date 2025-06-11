import { Heading } from "@/components";
import { getAuctionById } from "@/services";

import { AuctionForm } from "../../components";

export const Update = async ({ params }: { params: { id: string } }) => {
  const data = await getAuctionById(params.id);

  return (
    <div className="mx-auto max-w-[75%] shadow-lg p-10 bg-white rounded-lg">
      <Heading
        title="Update your auction"
        subtitle="Please update the details of your car"
      />
      <AuctionForm auction={data} />
    </div>
  );
};

export default Update;
