"use client";

import { Button } from "flowbite-react";
import Link from "next/link";

type Props = {
  id: string;
};

export const UpdateButton = ({ id }: Props) => (
  <Button outline>
    <Link href={`/auctions/update/${id}`}>Update Auction</Link>
  </Button>
);
