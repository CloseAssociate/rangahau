# project rangahau
This public project defines the interface between MoH Data and Digital system and Rangahau. The interface is a URL parameter constructed to pass survey participant
information between two systems. The interface is defined in the RS.Rangahau.Common.Participant project, and example usage can be found in RS.Rangahau.Participant.Tests.

This implementation is in C#, but the interface contract intended to be language agnostics.

AES encryption with the CBC cipher mode is used.  The IV is prepended to the cipher and intended to be sent atomically between the systems.
