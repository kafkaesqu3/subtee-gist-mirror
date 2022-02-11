#include <stdio.h>
#include <string.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <net/if.h>
#include <net/ethernet.h>
#include <netinet/in.h>
#include <netinet/ip.h>
#include <arpa/inet.h>
#include <netpacket/packet.h>
#include <linux/filter.h>

#define OP_LDH (BPF_LD  | BPF_H   | BPF_ABS)
#define OP_LDB (BPF_LD  | BPF_B   | BPF_ABS)
#define OP_JEQ (BPF_JMP | BPF_JEQ | BPF_K)
#define OP_RET (BPF_RET | BPF_K)

// Filter TCP segments to port 80
static struct sock_filter bpfcode[8] = {
        { OP_LDH, 0, 0, 12          },  // ldh [12]                                                                                                                                                                                                                         
        { OP_JEQ, 0, 5, ETH_P_IP    },  // jeq #0x800, L2, L8                                                                                                                                                                                                               
        { OP_LDB, 0, 0, 23          },  // ldb [23]           # 14 bytes of ethernet header + 9 bytes in IP header until the protocol                                                                                                                                       
        { OP_JEQ, 0, 3, IPPROTO_TCP },  // jeq #0x6, L4, L8                                                                                                                                                                                                                 
        { OP_LDH, 0, 0, 36          },  // ldh [36]           # 14 bytes of ethernet header + 20 bytes of IP header (we assume no options) + 2 bytes of offset until the port                                                                                               
        { OP_JEQ, 0, 1, 80          },  // jeq #0x50, L6, L8                                                                                                                                                                                                                
        { OP_RET, 0, 0, -1,         },  // ret #0xffffffff    # (accept)                                                                                                                                                                                                    
        { OP_RET, 0, 0, 0           },  // ret #0x0           # (reject)                                                                                                                                                                                                    

};

int main(int argc, char **argv)
{
	int sock;
	int n;
	char buf[2000];
	struct sockaddr_ll addr;
	struct packet_mreq mreq;
	struct iphdr *ip;
	char saddr_str[INET_ADDRSTRLEN], daddr_str[INET_ADDRSTRLEN];
	char *proto_str;
	char *name;
	struct sock_fprog bpf = { 8, bpfcode };

	if (argc != 2) {
		printf("Usage: %s ifname\n", argv[0]);
		return 1;
	}

	name = argv[1];

	sock = socket(AF_PACKET, SOCK_RAW, htons(ETH_P_ALL));
	if (sock < 0) {
		perror("socket");
		return 1;
	}

	memset(&addr, 0, sizeof(addr));
	addr.sll_ifindex = if_nametoindex(name);
	addr.sll_family = AF_PACKET;
	addr.sll_protocol = htons(ETH_P_ALL);

	if (bind(sock, (struct sockaddr *) &addr, sizeof(addr))) {
		perror("bind");
		return 1;
	}

	if (setsockopt(sock, SOL_SOCKET, SO_ATTACH_FILTER, &bpf, sizeof(bpf))) {
		perror("setsockopt ATTACH_FILTER");
		return 1;
	}

	memset(&mreq, 0, sizeof(mreq));
	mreq.mr_type = PACKET_MR_PROMISC;
	mreq.mr_ifindex = if_nametoindex(name);

	if (setsockopt(sock, SOL_PACKET,
				PACKET_ADD_MEMBERSHIP, (char *)&mreq, sizeof(mreq))) {
		perror("setsockopt MR_PROMISC");
		return 1;
	}

	for (;;) {
		n = recv(sock, buf, sizeof(buf), 0);
		if (n < 1) {
			perror("recv");
			return 0;
		}

		ip = (struct iphdr *)(buf + sizeof(struct ether_header));

		inet_ntop(AF_INET, &ip->saddr, saddr_str, sizeof(saddr_str));
		inet_ntop(AF_INET, &ip->daddr, daddr_str, sizeof(daddr_str));

		switch (ip->protocol) {
#define PTOSTR(_p,_str) \
			case _p: proto_str = _str; break

		PTOSTR(IPPROTO_ICMP, "icmp");
		PTOSTR(IPPROTO_TCP, "tcp");
		PTOSTR(IPPROTO_UDP, "udp");
		default:
			proto_str = "";
			break;
		}

		printf("IPv%d proto=%d(%s) src=%s dst=%s\n",
				ip->version, ip->protocol, proto_str, saddr_str, daddr_str);
	}

	return 0;
}